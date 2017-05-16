using System;

namespace IdentityServer4.LinqToDB
{
	public class TokenCleanupOptions
	{
		public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
	}
}