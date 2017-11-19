using System;

namespace IdentityServer4.Contrib.LinqToDB
{
	public class TokenCleanupOptions
	{
		public TimeSpan Interval { get; set; } = TimeSpan.FromSeconds(60);
	}
}