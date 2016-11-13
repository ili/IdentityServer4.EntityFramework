// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
    public class ClientClaim
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int ClientId { get; set; }
    }
}