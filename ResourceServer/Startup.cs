using System;
using AspNet.Security.OAuth.Introspection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ResourceServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(OAuthIntrospectionDefaults.AuthenticationScheme)
                .AddOAuthIntrospection(
                    options =>
                    {
                        // TODO: Use from environment variable. Configuration["IDENTITY_SERVER_URL"]
                        options.Authority = new Uri("http://127.0.0.1:5000");
                        // TODO: Use from environment variable. Configuration["CLIENT_ID"]
                        options.ClientId = "5035f951-f7bb-459d-b196-bb212292bb4d";
                        // TODO: Use from environment variable. Configuration["CLIENT_SECRET"]
                        options.ClientSecret = "89e43125-d963-4694-b770-096795a6e1e1";
                        options.RequireHttpsMetadata = false;
                    }
                );
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

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "api/{controller}/{action}");
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync(nameof(ResourceServer));
            });
        }
    }
}
