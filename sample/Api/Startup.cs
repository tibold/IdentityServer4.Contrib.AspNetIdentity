using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var scopePolicy = new AuthorizationPolicyBuilder()
                .RequireClaim("scope", "api1")
                .Build();

            services.AddMvcCore(setup =>
            {
                setup.Filters.Add(new AuthorizeFilter(scopePolicy));
            })
            .AddJsonFormatters()
            .AddAuthorization();
        }

        public void Configure(IApplicationBuilder app)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                Authority = "http://localhost:57384",
                Audience = "http://localhost:57384/resources",
                RequireHttpsMetadata = false,
                
                AutomaticAuthenticate = true,

                TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                }
            });

            app.UseMvc();
        }
    }
}
