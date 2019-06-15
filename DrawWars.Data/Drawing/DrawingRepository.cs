using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DrawWars.Data.Drawing
{
    public class DrawingRepository : BaseRepository, IDrawingRepository
    {
        public DrawingRepository(IConfiguration config) : base(config) { }

        public Drawing Insert(string url, Guid deviceID, Guid sessionID) =>
            ExecuteOnConnection(connection =>
                connection.Query<Drawing>(
                    sql: @"[dbo].[DrawingInsert]",
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        url,
                        deviceID,
                        sessionID
                    }
                ).SingleOrDefault()
            );

        public IEnumerable<Drawing> GetByDeviceID(Guid deviceID, int page, int pageSize) =>
            ExecuteOnConnection(connection =>
                connection.Query<Drawing>(
                    sql: @"[dbo].[DrawingGetByDeviceID]",
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        deviceID,
                        page,
                        pageSize
                    }
                ).ToList()
            );

        public IEnumerable<Drawing> GetBySessionID(Guid sessionID, int page, int pageSize) =>
            ExecuteOnConnection(connection =>
                connection.Query<Drawing>(
                    sql: @"[dbo].[DrawingGetBySessionID]",
                    commandType: CommandType.StoredProcedure,
                    param: new
                    {
                        sessionID,
                        page,
                        pageSize
                    }
                ).ToList()
            );

        public IEnumerable<Drawing> GetRandom() =>
            ExecuteOnConnection(connection =>
                connection.Query<Drawing>(
                    sql: @"[dbo].[DrawingGetRandom]",
                    commandType: CommandType.StoredProcedure
                ).ToList()
            );
    }
}
