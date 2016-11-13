using LinqToDB;
using LinqToDB.Data;

namespace IdentityServer4.LinqToDB.Interfaces
{
	public interface IDataConnectionFactory
	{
		DataContext GetContext();
		DataConnection GetConnection();
	}
}