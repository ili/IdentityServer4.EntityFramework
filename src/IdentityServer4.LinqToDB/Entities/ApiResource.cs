using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	///     Represents <see cref="Models.ApiResource" /> in database
	/// </summary>
	public class ApiResource : Models.ApiResource
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}