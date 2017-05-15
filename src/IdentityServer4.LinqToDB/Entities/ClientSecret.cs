using IdentityServer4.Models;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Represents <see cref="Models.Client.ClientSecrets"/> in database
	/// </summary>
	public class ClientSecret : Secret
	{
		/// <summary>
		/// <see cref="Models.Client.ClientId"/>
		/// </summary>
		public string ClientId { get; set; }
	}
}