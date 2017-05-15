using LinqToDB.Mapping;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Base class for storing secrets
	/// </summary>
	public abstract class Secret : Models.Secret
	{
		/// <summary>
		/// <see cref="int"/> identity field
		/// </summary>
		[PrimaryKey]
		[Identity]
		public int Id { get; set; }
	}
}