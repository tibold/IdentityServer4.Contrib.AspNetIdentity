// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IdentityServer4.Contrib.AspNetIdentity
{
    public class IdSrvSignInManager<TUser> : SignInManager<TUser>
        where TUser : class
    {
        private readonly IHttpContextAccessor contextAccessor;
        private readonly IdentityOptions options;

        public IdSrvSignInManager(
            UserManager<TUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<TUser>> logger)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger)
        {
            this.contextAccessor = contextAccessor;
            options = optionsAccessor.Value;
        }

        private HttpContext context
        {
            get
            {
                var context = contextAccessor?.HttpContext;
                if (context == null)
                {
                    throw new InvalidOperationException("HttpContext must not be null.");
                }
                return context;
            }
        }

        /// <summary>
        ///     Signs in the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user to sign-in.</param>
        /// <param name="authenticationProperties">Properties applied to the login and authentication cookie.</param>
        /// <param name="authenticationMethod">Name of the method used to authenticate the user.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public override async Task SignInAsync(TUser user, AuthenticationProperties authenticationProperties,
            string authenticationMethod = null)
        {
            var user_principal = await CreateUserPrincipalAsync(user);

            // Review: should we guard against CreateUserPrincipal returning null?
            user_principal.Identities.First().AddClaims(new[]
            {
                new Claim(JwtClaimTypes.IdentityProvider, authenticationMethod ?? Constants.BuiltInIdentityProvider),
                new Claim(JwtClaimTypes.AuthenticationTime, DateTime.UtcNow.ToEpochTime().ToString())
            });

            // Review: Do we need this from the original identity codebase?
            //if (authenticationMethod != null)
            //{
            //    user_principal.Identities.First().AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            //}

            await context.Authentication.SignInAsync(options.Cookies.ApplicationCookieAuthenticationScheme,
                user_principal,
                authenticationProperties ?? new AuthenticationProperties());
        }
    }
}