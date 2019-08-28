using Dapper.Contrib.Extensions;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DrawWars.Data
{
    public class BaseRepository<T>
        where T : BaseEntity
    {
        #region Private Data

        private readonly IConfiguration _config;

        #endregion

        public BaseRepository(IConfiguration config)
        {
            _config = config;
        }

        public T Get(int id)
        {
            return ExecuteOnConnection(connection => connection.Get<T>(id));
        }

        public T Create(T target)
        {
            return ExecuteOnConnection(connection =>
            {
                target.Id = (int)connection.Insert(target);

                return target;
            });
        }
        
        #region Protected Utils

        protected IDbConnection GetNewConnection() => new SqlConnection(_config.GetConnectionString("Default"));

        protected Out ExecuteOnConnection<Out>(Func<IDbConnection, Out> toExecute)
        {
            using (var connection = GetNewConnection())
            {
                connection.Open();

                return toExecute(connection);
            }
        }

        #endregion
    }
}
