// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer4.Contrib.AspNetIdentity.Tests.Clients.Setup
{
    static class Users
    {
        public static async Task Seed(this UserManager<IdentityUser> userManager)
        {
            var user = new IdentityUser
            {
                Id = "818727",
                UserName = "alice",
                Email = "AliceSmith@email.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user);
            await userManager.AddPasswordAsync(user, "alice");
            await userManager.AddToRolesAsync(user, new[] { "Admin", "Geek" });
            await userManager.AddClaimsAsync(user, new[]
            {
                new Claim(JwtClaimTypes.GivenName, "Alice"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://alice.com"),
                // NOTE: Microsoft.AspNetCore.Identity.EntityFrameworkCore does not care for the claim value type, so this will be treated as a string.
                new Claim(JwtClaimTypes.Address,
                    @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                    Constants.ClaimValueTypes.Json)
            });

            user = new IdentityUser
            {
                Id = "88421113",
                UserName = "bob",
                Email = "BobSmith@email.com",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user);
            await userManager.AddPasswordAsync(user, "bob");
            await userManager.AddToRolesAsync(user, new[] { "Developer", "Geek" });
            await userManager.AddClaimsAsync(user, new[]
            {
                new Claim(JwtClaimTypes.GivenName, "Bob"),
                new Claim(JwtClaimTypes.FamilyName, "Smith"),
                new Claim(JwtClaimTypes.WebSite, "http://bob.com"),
                // NOTE: Microsoft.AspNetCore.Identity.EntityFrameworkCore does not care for the claim value type, so this will be treated as a string.
                new Claim(JwtClaimTypes.Address,
                    @"{ 'street_address': 'One Hacker Way', 'locality': 'Heidelberg', 'postal_code': 69118, 'country': 'Germany' }",
                    Constants.ClaimValueTypes.Json)
            });
        }
    }
}