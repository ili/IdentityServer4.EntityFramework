using LinqToDB;
using LinqToDB.Data;

namespace IdentityServer4.EntityFramework.Interfaces
{
	public interface IDataConnectionFactory
	{
		DataContext GetContext();
		DataConnection GetConnection();
	}
}