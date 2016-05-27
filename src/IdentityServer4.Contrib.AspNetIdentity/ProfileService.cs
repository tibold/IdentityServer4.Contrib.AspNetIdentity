// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core.Extensions;
using IdentityServer4.Core.Models;
using IdentityServer4.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Contrib.AspNetIdentity
{
    public class ProfileService<TUser> : IProfileService
        where TUser : class
    {
        private readonly UserManager<TUser> userManager;

        public ProfileService(UserManager<TUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var user = await userManager.FindByIdAsync(context.Subject.GetSubjectId());

            var claims = await getClaims(user);

            if (!context.AllClaimsRequested)
            {
                claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();
            }

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Subject == null) throw new ArgumentNullException(nameof(context.Subject));

            context.IsActive = false;

            var subject = context.Subject;
            var user = await userManager.FindByIdAsync(context.Subject.GetSubjectId());

            if (user != null)
            {
                var security_stamp_changed = false;

                if (userManager.SupportsUserSecurityStamp)
                {
                    var security_stamp = (
                        from claim in subject.Claims
                        where claim.Type == "security_stamp"
                        select claim.Value
                        ).SingleOrDefault();

                    if (security_stamp != null)
                    {
                        var latest_security_stamp = await userManager.GetSecurityStampAsync(user);
                        security_stamp_changed = security_stamp != latest_security_stamp;
                    }
                }

                context.IsActive =
                    !security_stamp_changed &&
                    !await userManager.IsLockedOutAsync(user);
            }
        }

        private async Task<List<Claim>> getClaims(TUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, await userManager.GetUserIdAsync(user)),
                new Claim(JwtClaimTypes.Name, await userManager.GetUserNameAsync(user))
            };

            if (userManager.SupportsUserEmail)
            {
                var email = await userManager.GetEmailAsync(user);
                if (!string.IsNullOrWhiteSpace(email))
                {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.Email, email),
                        new Claim(JwtClaimTypes.EmailVerified,
                            await userManager.IsEmailConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            if (userManager.SupportsUserPhoneNumber)
            {
                var phoneNumber = await userManager.GetPhoneNumberAsync(user);
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    claims.AddRange(new[]
                    {
                        new Claim(JwtClaimTypes.PhoneNumber, phoneNumber),
                        new Claim(JwtClaimTypes.PhoneNumberVerified,
                            await userManager.IsPhoneNumberConfirmedAsync(user) ? "true" : "false", ClaimValueTypes.Boolean)
                    });
                }
            }

            if (userManager.SupportsUserClaim)
            {
                claims.AddRange(await userManager.GetClaimsAsync(user));
            }

            if (userManager.SupportsUserRole)
            {
                var roles = await userManager.GetRolesAsync(user);
                claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));
            }

            return claims;
        }
    }
}