using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.Client.IdentityProviderRestrictions" /> in database
	/// </summary>
	public class ClientIdentityProviderRestrictions
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     Represents provider restrictions
		/// </summary>
		public string Provider { get; set; }

		/// <summary>
		///     <see cref="Models.Client.ClientId" />
		/// </summary>
		public string ClientId { get; set; }
	}
}