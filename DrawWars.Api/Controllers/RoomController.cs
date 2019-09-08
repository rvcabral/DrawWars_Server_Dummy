using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DrawWars.Api.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : Controller
    {
        private IThemeRepository _themeRepository;

        private IPlayerRepository _playerRepository;

        private IGameRoomRepository _roomRepository;

        private IDrawingRepository _drawingRepository;

        private IPlayerScoreRepository _scoreRepository;
        
        public RoomController(IGameRoomRepository roomRepo, IPlayerScoreRepository scoreRepo, IPlayerRepository playerRepo, IDrawingRepository drawingRepo, IThemeRepository themeRepo)
        {
            _roomRepository = roomRepo;
            _themeRepository = themeRepo;
            _scoreRepository = scoreRepo;
            _playerRepository = playerRepo;
            _drawingRepository = drawingRepo;
        }
        
        public List<GameRoom> GetAll(int page = 0, int pageSize = 10)
        {
            return _roomRepository.List(page, pageSize);
        }

        [Route("{roomId}/scores")]
        public IList GetRoomScores(int roomId)  
        {
            return _scoreRepository
                .GetByGameRoom(roomId)
                .Select(ps => new
                {
                    ps.GameRoomId,
                    ps.PlayerId,
                    _playerRepository.Get(ps.PlayerId)?.Name,
                    ps.Score
                })
                .ToList();
        }

        [Route("{roomId}/drawings")]
        public IList GetRoomDrawings(int roomId)
        {
            return _drawingRepository
                .ListByGameRoom(roomId)
                .Select(d => new
                {
                    d.GameRoomId,
                    d.PlayerId,
                    _playerRepository.Get(d.PlayerId)?.Name,
                    d.ThemeId,
                    _themeRepository.Get(d.ThemeId).Text,
                    d.Url
                })
                .ToList();
        }
    }
}
