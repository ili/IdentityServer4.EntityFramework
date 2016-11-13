// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using LinqToDB.Mapping;
using static IdentityServer4.IdentityServerConstants;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ClientSecret
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public DateTime? Expiration { get; set; }
        public string Type { get; set; } = SecretTypes.SharedSecret;
        public int ClientId { get; set; }
    }
}