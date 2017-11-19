using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.Client.AllowedGrantTypes" /> in database
	/// </summary>
	public class ClientGrantType
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     Grant type value
		/// </summary>
		public string GrantType { get; set; }

		/// <summary>
		///     <see cref="Models.Client.ClientId" />
		/// </summary>
		public string ClientId { get; set; }
	}
}