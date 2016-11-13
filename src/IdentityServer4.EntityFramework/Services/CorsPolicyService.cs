// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Threading.Tasks;
using IdentityServer4.Services;
using System.Linq;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.EntityFramework.Interfaces;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.EntityFramework.Services
{
    public class CorsPolicyService : ICorsPolicyService
    {
        private readonly IDataConnectionFactory _dataConnectionFactory;
        private readonly ILogger<CorsPolicyService> _logger;

        public CorsPolicyService(IDataConnectionFactory dataConnectionFactory, ILogger<CorsPolicyService> logger)
        {
            if (dataConnectionFactory == null) throw new ArgumentNullException(nameof(dataConnectionFactory));

            _dataConnectionFactory = dataConnectionFactory;
            _logger = logger;
        }

        public Task<bool> IsOriginAllowedAsync(string origin)
        {
	        var db = _dataConnectionFactory.GetContext();

	        var isAllowed = db.GetTable<ClientCorsOrigin>()
		        .Any(_ => _.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase));

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return Task.FromResult(isAllowed);
        }
    }
}