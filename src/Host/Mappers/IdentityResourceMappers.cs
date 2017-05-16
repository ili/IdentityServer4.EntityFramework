// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.LinqToDB.Mappers
{
	public static class IdentityResourceMappers
	{
		static IdentityResourceMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<IdentityResourceMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		public static IdentityResource ToModel(this Entities.IdentityResource resource)
		{
			return resource == null ? null : Mapper.Map<IdentityResource>(resource);
		}

		public static Entities.IdentityResource ToEntity(this IdentityResource resource)
		{
			return resource == null ? null : Mapper.Map<Entities.IdentityResource>(resource);
		}
	}
}