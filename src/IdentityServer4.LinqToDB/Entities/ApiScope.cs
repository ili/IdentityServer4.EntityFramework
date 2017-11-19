using IdentityServer4.Models;
using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.ApiResource.Scopes" /> in database
	/// </summary>
	public class ApiScope : Scope
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     <see cref="ApiResource.Id" />
		/// </summary>
		public int ApiResourceId { get; set; }
	}
}