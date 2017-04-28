using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using IdentityServer4.LinqToDB.Interfaces;
using IdentityServer4.Models;
using LinqToDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.LinqToDB
{
	internal class TokenCleanup
	{
		private readonly TimeSpan _interval;
		private readonly ILogger<TokenCleanup> _logger;
		private readonly IServiceProvider _serviceProvider;
		private CancellationTokenSource _source;

		public TokenCleanup(IServiceProvider serviceProvider, ILogger<TokenCleanup> logger, TimeSpan interval)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
			_interval = interval;
		}

		public void Start()
		{
			if (_source != null) throw new InvalidOperationException("Already started. Call Stop first.");

			_logger.LogDebug("Starting token cleanup");

			_source = new CancellationTokenSource();
			Task.Factory.StartNew(() => Start(_source.Token));
		}

		public void Stop()
		{
			if (_source == null) throw new InvalidOperationException("Not started. Call Start first.");

			_logger.LogDebug("Stopping token cleanup");

			_source.Cancel();
			_source = null;
		}

		private async Task Start(CancellationToken cancellationToken)
		{
			while (true)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogDebug("CancellationRequested");
					break;
				}

				try
				{
					await Task.Delay(_interval, cancellationToken);
				}
				catch
				{
					_logger.LogDebug("Task.Delay exception. exiting.");
					break;
				}

				if (cancellationToken.IsCancellationRequested)
				{
					_logger.LogDebug("CancellationRequested");
					break;
				}

				ClearTokens();
			}
		}

		private void ClearTokens()
		{
			try
			{
				_logger.LogTrace("Querying for tokens to clear");

				using (var serviceScope = _serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
				{
					var context = serviceScope.ServiceProvider.GetService<IDataConnectionFactory>();

					{
						var expired = context.GetContext()
							.GetTable<PersistedGrant>()
							.Where(x => x.Expiration < DateTimeOffset.UtcNow)
							.Delete();

						_logger.LogDebug("Clearing {tokenCount} tokens", expired);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError("Exception cleaning tokens {exception}", ex.Message);
			}
		}
	}
}