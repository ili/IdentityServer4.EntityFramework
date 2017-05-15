using IdentityServer4.Models;
using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Represents <see cref="Models.Client.AllowedCorsOrigins"/> in database
	/// </summary>
	public class ClientCorsOrigin
	{
		/// <summary>
		/// <see cref="int"/> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		/// Origin value
		/// </summary>
		public string Origin { get; set; }

		/// <summary>
		/// <see cref="Models.Client.ClientId"/>
		/// </summary>
		public string ClientId { get; set; }
	}
}