// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.IO;
using System.Security.Cryptography.X509Certificates;
using IdentityServer4.Contrib.AspNetIdentity.Tests.Clients.Setup.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.PlatformAbstractions;

namespace IdentityServer4.Contrib.AspNetIdentity.Tests.Clients.Setup
{
    // This project can output the Class library as a NuGet Package.
    // To enable this option, right-click on the project and select the Properties menu item. In the Build tab select "Produce outputs on build".
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var inMemorySqlite = new SqliteConnection("Data Source=:memory:");
            inMemorySqlite.Open();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddEntityFramework()
                .AddEntityFrameworkSqlite()
                .AddDbContext<IdentityContext>(options => { options.UseSqlite(inMemorySqlite); });

            var cert =
                new X509Certificate2(
                    Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "idsrvtest.pfx"),
                    "idsrv3test");

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                // Tests use dummy passwords
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 1;
            })
                .AddEntityFrameworkStores<IdentityContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
            {
                options.SigningCertificate = cert;
                options.IssuerUri = "https://idsrv4";
            })
                .AddInMemoryClients(Clients.Get())
                .AddInMemoryScopes(Scopes.Get())
                .ConfigureAspNetIdentity<IdentityUser>()
                .AddCustomGrantValidator<CustomGrantValidator>()
                .AddCustomGrantValidator<CustomGrantValidator2>();
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            app.UseIdentity();
            app.UseIdentityServer();

            // Apply migrations to the in memory database
            var context = app.ApplicationServices.GetRequiredService<IdentityContext>();
            context.Database.OpenConnection();
            context.Database.Migrate();

            var role_manager = app.ApplicationServices.GetRequiredService<RoleManager<IdentityRole>>();
            role_manager.Seed().Wait();

            var user_manager = app.ApplicationServices.GetRequiredService<UserManager<IdentityUser>>();
            user_manager.Seed().Wait();
        }
    }
}