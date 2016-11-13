// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Interfaces;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Stores
{
    public class ClientStore : IClientStore
    {
        private readonly IDataConnectionFactory _dataConnectionFactory;
        private readonly ILogger<ClientStore> _logger;

        public ClientStore(IDataConnectionFactory dataConnectionFactory, ILogger<ClientStore> logger)
        {
            if (dataConnectionFactory == null) throw new ArgumentNullException(nameof(dataConnectionFactory));

            _dataConnectionFactory = dataConnectionFactory;
            _logger = logger;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _dataConnectionFactory.GetContext().GetTable<Entities.Client>()
                .LoadWith(x => x.AllowedGrantTypes)
                .LoadWith(x => x.RedirectUris)
                .LoadWith(x => x.PostLogoutRedirectUris)
                .LoadWith(x => x.AllowedScopes)
                .LoadWith(x => x.ClientSecrets)
                .LoadWith(x => x.Claims)
                .LoadWith(x => x.IdentityProviderRestrictions)
                .LoadWith(x => x.AllowedCorsOrigins)
                .FirstOrDefault(x => x.ClientId == clientId);

            var model = client?.ToModel();

            _logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, model != null);

            return Task.FromResult(model);
        }
    }
}