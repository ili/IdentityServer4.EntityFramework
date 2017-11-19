// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using IdentityServer4.Models;
using IdentityServer4.Contrib.LinqToDB;

namespace IdentityServer4.LinqToDB.Mappers
{
	public static class ApiResourceMappers
	{
		static ApiResourceMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ApiResourceMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		public static ApiResource ToModel(this Contrib.LinqToDB.Entities.ApiResource resource)
		{
			return resource == null ? null : Mapper.Map<ApiResource>(resource);
		}

		public static Contrib.LinqToDB.Entities.ApiResource ToEntity(this ApiResource resource)
		{
			return resource == null ? null : Mapper.Map<Contrib.LinqToDB.Entities.ApiResource>(resource);
		}
	}
}