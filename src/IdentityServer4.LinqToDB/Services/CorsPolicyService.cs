// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.Services;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.LinqToDB.Services
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
		        .Any(_ => _.Origin.Equals(origin));

            _logger.LogDebug("Origin {origin} is allowed: {originAllowed}", origin, isAllowed);

            return Task.FromResult(isAllowed);
        }
    }
}