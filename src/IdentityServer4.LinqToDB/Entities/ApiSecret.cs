using IdentityServer4.Models;

namespace IdentityServer4.LinqToDB.Entities
{
	/// <summary>
	/// Represents <see cref="Models.ApiResource.ApiSecrets"/> in database
	/// </summary>
	public class ApiSecret : Secret
	{
		/// <summary>
		/// <see cref="ApiResource.Id"/>
		/// </summary>
		public int ApiResourceId { get; set; }
	}
}