// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using AutoMapper;
using IdentityServer4.Models;

namespace IdentityServer4.LinqToDB.Mappers
{
	public static class ClientMappers
	{
		static ClientMappers()
		{
			Mapper = new MapperConfiguration(cfg => cfg.AddProfile<ClientMapperProfile>())
				.CreateMapper();
		}

		internal static IMapper Mapper { get; }

		public static Client ToModel(this Contrib.LinqToDB.Entities.Client client)
		{
			return Mapper.Map<Client>(client);
		}

		public static Contrib.LinqToDB.Entities.Client ToEntity(this Client client)
		{
			return Mapper.Map<Contrib.LinqToDB.Entities.Client>(client);
		}
	}
}