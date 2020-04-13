using MySql.Data.MySqlClient;
using Npgsql;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Data.SqlClient;

namespace NetCoreServer.DataLoaders
{
    public class DatabaseConnectionFactory
    {
        /// <summary>
        /// Get connection based on property in settings
        /// </summary>
        /// <param name="connectionString">Connection string</param>
        /// <param name="type">Type of database. Set in property. Can be "mysql", "mssql", "postgresql","oracle"</param>
        /// <returns>Connection to database</returns>
        public IDbConnection GetDbConnection(string connectionString, string type)
        {
            switch (type)
            {
                case "mysql":
                    {
                        return new MySqlConnection(connectionString);
                    }
                case "mssql":
                    {
                        return new SqlConnection(connectionString);
                    }
                case "postgresql":
                    {
                        return new NpgsqlConnection(connectionString);
                    }
                case "oracle":
                    {
                        return new OracleConnection(connectionString);
                    }
                default: return null;
            }
        }
    }
}