using System;
using System.Collections.Generic;

namespace DrawWars.Data.Drawing
{
    public interface IDrawingRepository
    {
        Drawing Insert(string url, Guid deviceID, Guid sessionID);

        IEnumerable<Drawing> GetRandom();

        IEnumerable<Drawing> GetBySessionID(Guid sessionID, int page, int pageSize);

        IEnumerable<Drawing> GetByDeviceID(Guid deviceID, int page, int pageSize);
    }
}
