// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
    public class ClientScope
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string Scope { get; set; }
        public int ClientId { get; set; }
    }
}