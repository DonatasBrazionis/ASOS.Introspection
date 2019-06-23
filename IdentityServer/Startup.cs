using System;
using AspNet.Security.OAuth.Validation;
using IdentityServer.Contracts;
using IdentityServer.Providers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services
                .AddAuthentication(OAuthValidationDefaults.AuthenticationScheme)
                .AddOAuthValidation()
                .AddOpenIdConnectServer(options =>
                {
                    options.ProviderType = typeof(AuthorizationProvider);
                    options.TokenEndpointPath = "/connect/token";
                    options.LogoutEndpointPath = "/connect/logout";
                    options.AccessTokenLifetime = HostingEnvironment.IsDevelopment() ? new TimeSpan(8, 0, 0) : new TimeSpan(0, 5, 0);
                    options.RefreshTokenLifetime = new TimeSpan(1, 0, 0);
                    options.AllowInsecureHttp = true;
                    options.IntrospectionEndpointPath = "/connect/introspect";
                });

            services.AddScoped<AuthorizationProvider>();
            services.AddScoped<IDatabaseProvider, DatabaseProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin();
                builder.AllowCredentials();
                builder.AllowAnyMethod();
                builder.AllowAnyHeader();
            });

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}");
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(nameof(IdentityServer));
            });
        }
    }
}
