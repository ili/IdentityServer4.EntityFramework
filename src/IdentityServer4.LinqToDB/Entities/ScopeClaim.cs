// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using LinqToDB.Mapping;

namespace IdentityServer4.EntityFramework.Entities
{
    public class ScopeClaim
    {
		[PrimaryKey, Identity]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool AlwaysIncludeInIdToken { get; set; }
        public int ScopeId { get; set; }
    }
}