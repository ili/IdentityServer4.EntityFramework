using IdentityServer4.LinqToDB.Entities;
using IdentityServer4.LinqToDB.Interfaces;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.DataProvider.SQLite;
using Microsoft.Data.Sqlite;

namespace Host
{
	internal class DataConnectionFactory : IDataConnectionFactory
	{
		private static IDataConnectionFactory _instance;
		private readonly SqliteConnection _connection;
		private readonly string _connectionString;

		private DataConnectionFactory()
		{
			MappingExtensions.ApplyDefaultEntitiesMappings();

			_connectionString = "Data Source=file:memdb?mode=memory&cache=shared";

			_connection = new SqliteConnection(_connectionString);
			_connection.Open();
		}

		public static IDataConnectionFactory Instance => _instance ?? (_instance = new DataConnectionFactory());

		public DataContext GetContext()
		{
			return new DataContext(SQLiteTools.GetDataProvider(), _connectionString);
		}

		public DataConnection GetConnection()
		{
			return new DataConnection(SQLiteTools.GetDataProvider(), _connectionString);
		}
	}
}