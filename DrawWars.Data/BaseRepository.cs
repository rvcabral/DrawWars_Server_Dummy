using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DrawWars.Data
{
    public class BaseRepository
    {
        private readonly IConfiguration _config;

        public BaseRepository(IConfiguration config)
        {
            _config = config;
        }

        protected IDbConnection GetNewConnection() => new SqlConnection(_config.GetConnectionString("Default"));

        protected T ExecuteOnConnection<T>(Func<IDbConnection, T> toExecute)
        {
            using (var connection = GetNewConnection())
            {
                connection.Open();

                return toExecute(connection);
            }
        }
    }
}
