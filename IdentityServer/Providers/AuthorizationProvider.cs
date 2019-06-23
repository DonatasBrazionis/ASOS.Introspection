using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Extensions;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using IdentityServer.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;

namespace IdentityServer.Providers
{
    /// <summary>
    /// Resource owner password credentials (ROPC) flow implementation using AspNet.Security.OpenIdConnect.Server (ASOS).
    /// </summary>
    public class AuthorizationProvider : OpenIdConnectServerProvider
    {
        public AuthorizationProvider(IDatabaseProvider databaseProvider)
        {
            DatabaseProvider = databaseProvider;
        }

        protected IDatabaseProvider DatabaseProvider { get; set; }

        // Validate the grant_type and the client application credentials.
        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            // Reject the token requests that don't use grant_type=password or grant_type=refresh_token.
            if (!context.Request.IsPasswordGrantType() && !context.Request.IsRefreshTokenGrantType())
            {
                // Handle response ourselves and return an object.
                context.HandleResponse();
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = 400;
                await context.HttpContext.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new
                        {
                            error = OpenIdConnectConstants.Errors.UnsupportedGrantType,
                                error_description = "Only grant_type=password or grant_type=refresh_token are accepted by this server."
                        }));
                return;
            }

            // Check if refresh-token exists in DB.
            if (context.Request.IsRefreshTokenGrantType())
            {
                if (!await DatabaseProvider.IsRefreshTokenExists(context.Request.RefreshToken))
                {
                    // Handle response ourselves and return an object.
                    context.HandleResponse();
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = 400;
                    await context.HttpContext.Response.WriteAsync(
                        JsonConvert.SerializeObject(
                            new
                            {
                                error = OpenIdConnectConstants.Errors.InvalidClient,
                                    error_description = "Invalid client."
                            }));
                    return;
                }
            }

            context.Skip();
            return;
        }

        // Implementing HandleTokenRequest to issue an authentication ticket containing the user claims.
        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {
            // Only handle grant_type=password requests and let ASOS
            // process grant_type=refresh_token requests automatically.
            if (context.Request.IsPasswordGrantType())
            {
                var email = context.Request.Username.ToLower();

                // Get user login data.
                var user = await DatabaseProvider.GetUserByEmail(email);
                if (user == null || user.Email.ToLower() != email)
                {
                    // Handle response ourselves and return an object.
                    context.HandleResponse();
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = 400;
                    await context.HttpContext.Response.WriteAsync(
                        JsonConvert.SerializeObject(
                            new
                            {
                                error = OpenIdConnectConstants.Errors.InvalidGrant,
                                    error_description = "Invalid credentials."
                            }));
                    return;
                }

                if (!BCrypt.Net.BCrypt.Verify(context.Request.Password, user.PasswordHash))
                {
                    // Handle response ourselves and return an object.
                    context.HandleResponse();
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = 400;
                    await context.HttpContext.Response.WriteAsync(
                        JsonConvert.SerializeObject(
                            new
                            {
                                error = OpenIdConnectConstants.Errors.InvalidGrant,
                                    error_description = "Invalid credentials."
                            }));
                    return;
                }

                var identity = new ClaimsIdentity(OpenIdConnectServerDefaults.AuthenticationScheme);

                // Note: the subject claim is always included in both identity and
                // access tokens, even if an explicit destination is not specified.
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, user.UserId);
                identity.AddClaim(OpenIdConnectConstants.Claims.Name, user.UserId, OpenIdConnectConstants.Destinations.AccessToken);
                identity.AddClaim(ClaimTypes.Name, user.UserId, OpenIdConnectConstants.Destinations.AccessToken);
                identity.AddClaim(ClaimTypes.Email, user.Email, OpenIdConnectConstants.Destinations.AccessToken);

                // Create a new authentication ticket holding the user identity.
                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties(),
                    OpenIdConnectServerDefaults.AuthenticationScheme);

                // Set the list of scopes granted to the client application.
                // (specify offline_access to issue a refresh token).
                ticket.SetScopes(
                    OpenIdConnectConstants.Scopes.OpenId,
                    OpenIdConnectConstants.Scopes.OfflineAccess
                );

                context.Validate(ticket);
            }
            return;
        }

        // Save refresh-token.
        public override async Task ApplyTokenResponse(ApplyTokenResponseContext context)
        {
            if (context.Response.Error == null && context.Response.RefreshToken != null)
            {
                if (context.Request.IsRefreshTokenGrantType())
                {
                    await DatabaseProvider.DeleteRefreshToken(context.Request.RefreshToken);
                }

                var remoteIpAddressStringValues = context.HttpContext.Request.Headers["X-Forwarded-For"];
                var remoteIpAddress = remoteIpAddressStringValues.ToString();
                if (StringValues.IsNullOrEmpty(remoteIpAddressStringValues))
                {
                    remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                }

                string userAgent = null;
                if (context.HttpContext.Request.Headers.ContainsKey("User-Agent"))
                {
                    userAgent = context.HttpContext.Request.Headers["User-Agent"].ToString();
                }

                var userId = context?.Ticket?.Principal?.Identity?.Name;
                await DatabaseProvider.CreateRefreshToken(
                    userId,
                    context.Response.RefreshToken,
                    remoteIpAddress,
                    userAgent,
                    context.Options.RefreshTokenLifetime);
            }
            return;
        }

        // Logout, delete refresh-token.
        public override async Task HandleLogoutRequest(HandleLogoutRequestContext context)
        {
            if (context.Request.IsRefreshTokenGrantType())
            {
                if (!await DatabaseProvider.IsRefreshTokenExists(context.Request.RefreshToken))
                {
                    // Handle response ourselves and return an object
                    context.HandleResponse();
                    context.HttpContext.Response.ContentType = "application/json";
                    context.HttpContext.Response.StatusCode = 400;
                    await context.HttpContext.Response.WriteAsync(
                        JsonConvert.SerializeObject(
                            new
                            {
                                error = OpenIdConnectConstants.Errors.InvalidToken,
                                    error_description = "Invalid token."
                            }));
                    return;
                }
                await DatabaseProvider.DeleteRefreshToken(context.Request.RefreshToken);
                context.HandleResponse();
            }
        }

        // Validate introspection requests with client-ids and client-secrets.
        public override async Task ValidateIntrospectionRequest(ValidateIntrospectionRequestContext context)
        {
            // Reject the token requests that don't use client_id and client_secret.
            if (string.IsNullOrWhiteSpace(context.ClientId) || string.IsNullOrWhiteSpace(context.ClientSecret))
            {
                // Handle response ourselves and return an object.
                context.HandleResponse();
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = 400;
                await context.HttpContext.Response.WriteAsync(
                    JsonConvert.SerializeObject(
                        new
                        {
                            error = OpenIdConnectConstants.Errors.InvalidClient,
                                error_description = "Invalid client."
                        }));
                return;
            }
            if (await DatabaseProvider.IsClientExists(context.ClientId, context.ClientSecret))
            {
                context.Validate();
            }
            return;
        }

    }
}
