using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.IdentityResource" /> in database
	/// </summary>
	public class IdentityResource : Models.IdentityResource
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}