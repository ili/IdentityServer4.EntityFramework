using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.Client.RedirectUris" /> in database
	/// </summary>
	public class ClientRedirectUri
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     Represents redirect Uri
		/// </summary>
		public string RedirectUri { get; set; }

		/// <summary>
		///     <see cref="Models.Client.ClientId" />
		/// </summary>
		public string ClientId { get; set; }
	}
}