// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core;
using IdentityServer4.Core.Configuration;
using IdentityServer4.Core.Services;
using IdentityServer4.Core.Validation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer4.Contrib.AspNetIdentity
{
    public static class StartupExtensions
    {
        public static IIdentityServerBuilder ConfigureAspNetIdentity<TUser>(this IIdentityServerBuilder builder)
            where TUser : class
        {
            var services = builder.Services;

            services.AddTransient<SignInManager<TUser>, IdSrvSignInManager<TUser>>();
            services.AddTransient<IProfileService, ProfileService<TUser>>();
            services.AddTransient<IResourceOwnerPasswordValidator, ResourceOwnerPasswordValidator<TUser>>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.AuthenticationScheme = Constants.PrimaryAuthenticationType;

                options.Cookies.ApplicationCookie.LoginPath = new PathString("/" + Constants.RoutePaths.Login);
                options.Cookies.ApplicationCookie.LogoutPath = new PathString("/" + Constants.RoutePaths.Logout);

                options.ClaimsIdentity.UserIdClaimType = JwtClaimTypes.Subject;
                options.ClaimsIdentity.UserNameClaimType = JwtClaimTypes.Name;
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            services.Configure<IdentityServerOptions>(
                options =>
                {
                    options.AuthenticationOptions.PrimaryAuthenticationScheme = Constants.PrimaryAuthenticationType;
                });

            return builder;
        }
    }
}