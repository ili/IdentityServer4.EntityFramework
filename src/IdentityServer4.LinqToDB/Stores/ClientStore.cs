using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.Stores;
using LinqToDB;
using Microsoft.Extensions.Logging;
using Client = IdentityServer4.Models.Client;
using Secret = IdentityServer4.Models.Secret;

namespace IdentityServer4.LinqToDB.Stores
{
	public class ClientStore : IClientStore
	{
		private readonly IDataConnectionFactory _dataConnectionFactory;
		private readonly ILogger<ClientStore> _logger;

		public ClientStore(IDataConnectionFactory dataConnectionFactory, ILogger<ClientStore> logger)
		{
			_dataConnectionFactory = dataConnectionFactory ?? throw new ArgumentNullException(nameof(dataConnectionFactory));
			_logger = logger;
		}

		public Task<Client> FindClientByIdAsync(string clientId)
		{
			using (var db = _dataConnectionFactory.GetConnection())
			{
				var client = _dataConnectionFactory.GetContext()
					.GetTable<Entities.Client>()
					.FirstOrDefault(x => x.ClientId == clientId);

				if (client != null)
				{
					client.AllowedGrantTypes = db.GetTable<ClientGrantType>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.GrantType)
						.AsEnumerable();

					client.RedirectUris = db.GetTable<ClientRedirectUri>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.RedirectUri)
						.ToList();

					client.PostLogoutRedirectUris = db.GetTable<ClientPostLogoutRedirectUri>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.PostLogoutRedirectUri)
						.ToList();

					client.AllowedScopes = db.GetTable<ClientScope>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.Scope)
						.ToList();

					client.ClientSecrets = db.GetTable<ClientSecret>()
						.Where(_ => _.ClientId == clientId)
						.AsEnumerable()
						.Cast<Secret>()
						.ToList();

					client.Claims = db.GetTable<ClientClaim>()
						.Where(_ => _.ClientId == clientId)
						.AsEnumerable()
						.Cast<Claim>()
						.ToList();

					client.IdentityProviderRestrictions = db.GetTable<ClientIdentityProviderRestrictions>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.Provider)
						.ToList();

					client.AllowedCorsOrigins = db.GetTable<ClientCorsOrigin>()
						.Where(_ => _.ClientId == clientId)
						.Select(_ => _.Origin)
						.ToList();
				}

				_logger.LogDebug("{clientId} found in database: {clientIdFound}", clientId, client != null);

				return Task.FromResult((Client) client);
			}
		}
	}
}