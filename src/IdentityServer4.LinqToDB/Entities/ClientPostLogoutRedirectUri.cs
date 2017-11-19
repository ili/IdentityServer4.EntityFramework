using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.Client.PostLogoutRedirectUris" /> in database
	/// </summary>
	public class ClientPostLogoutRedirectUri
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     Represents post logout redirect Uri
		/// </summary>
		public string PostLogoutRedirectUri { get; set; }

		/// <summary>
		///     <see cref="Models.Client.ClientId" />
		/// </summary>
		public string ClientId { get; set; }
	}
}