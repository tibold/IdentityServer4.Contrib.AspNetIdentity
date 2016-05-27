// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IdentityServer4.Contrib.AspNetIdentity.Tests.Clients.Setup
{
    static class Roles
    {
        public static async Task Seed(this RoleManager<IdentityRole> roleManager)
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
            await roleManager.CreateAsync(new IdentityRole("Developer"));
            await roleManager.CreateAsync(new IdentityRole("Geek"));
        }
    }
}