// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LinqToDB.Mapping;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ClientCorsOrigin
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string Origin { get; set; }
        public int ClientId { get; set; }
    }
}