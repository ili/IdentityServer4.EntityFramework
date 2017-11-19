using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Contrib.LinqToDB.Entities;
using IdentityServer4.Contrib.LinqToDB.Interfaces;
using IdentityServer4.Services;
using LinqToDB;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Contrib.LinqToDB.Services
{
	public class CorsPolicyService : ICorsPolicyService
	{
		private readonly IDataConnectionFactory _dataConnectionFactory;
		private readonly ILogger<CorsPolicyService> _logger;

		public CorsPolicyService(IDataConnectionFactory dataConnectionFactory, ILogger<CorsPolicyService> logger)
		{
			_dataConnectionFactory = dataConnectionFactory ?? throw new ArgumentNullException(nameof(dataConnectionFactory));
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