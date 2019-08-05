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

        public async Task<T> GetAsync(int id)
        {
            return await ExecuteOnConnection(async connection => await connection.GetAsync<T>(id));
        }

        public async Task<T> CreateAsync(T target)
        {
            return await ExecuteOnConnection(async connection =>
            {
                target.Id = await connection.InsertAsync(target);

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
