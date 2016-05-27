// Copyright (c) Tibold Kandrai. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Core.Validation;
using Microsoft.AspNetCore.Identity;

namespace IdentityServer4.Contrib.AspNetIdentity
{
    public class ResourceOwnerPasswordValidator<TUser> : IResourceOwnerPasswordValidator
        where TUser : class
    {
        private readonly UserManager<TUser> userManager;

        public ResourceOwnerPasswordValidator(UserManager<TUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<CustomGrantValidationResult> ValidateAsync(string userName, string password,
            ValidatedTokenRequest request)
        {
            var user = await userManager.FindByNameAsync(userName);
            if (user != null && await userManager.CheckPasswordAsync(user, password))
            {
                return new CustomGrantValidationResult(await userManager.GetUserIdAsync(user), "password");
            }
            return new CustomGrantValidationResult("Invalid username or password");
        }
    }
}