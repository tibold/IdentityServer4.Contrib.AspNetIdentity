// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using IdentityServer4.Core.Models;

namespace IdentityServer4.Contrib.AspNetIdentity.Tests.Clients.Setup
{
    class Scopes
    {
        public static IEnumerable<Scope> Get()
        {
            return new List<Scope>
            {
                StandardScopes.OpenId,
                StandardScopes.Email,
                StandardScopes.OfflineAccess,
                StandardScopes.Address,
                new Scope
                {
                    Name = "api1",
                    Type = ScopeType.Resource,
                    ScopeSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    }
                },
                new Scope
                {
                    Name = "api2",
                    Type = ScopeType.Resource
                },
                new Scope
                {
                    Name = "api3",
                    Type = ScopeType.Resource
                }
            };
        }
    }
}