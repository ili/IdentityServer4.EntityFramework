using LinqToDB.Mapping;

namespace IdentityServer4.Contrib.LinqToDB.Entities
{
	/// <summary>
	///     Base class for storing claims
	/// </summary>
	public abstract class UserClaim
	{
		/// <summary>
		///     <see cref="int" /> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }

		/// <summary>
		///     Claim's Type
		/// </summary>
		public string Type { get; set; }
	}
}