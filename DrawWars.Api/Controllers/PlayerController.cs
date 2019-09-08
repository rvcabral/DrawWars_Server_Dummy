using DrawWars.Api.Utils;
using DrawWars.Data.Contracts;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Linq;

namespace DrawWars.Api.Controllers
{
    [ApiController]
    [Route("api/player")]
    public class PlayerController : Controller
    {
        private IThemeRepository _themeRepository;

        private IDrawingRepository _drawingRepository;

        private IPlayerScoreRepository _scoreRepository;

        public PlayerController(IPlayerScoreRepository scoreRepo, IDrawingRepository drawingRepo, IThemeRepository themeRepo)
        {
            _scoreRepository = scoreRepo;
            _themeRepository = themeRepo;
            _drawingRepository = drawingRepo;
        }
        
        [Route("drawings")]
        public IList Drawings(int page = 0, int pageSize = 10)
        {
            if(!Request.Headers.Keys.Contains("x-drawwars-auth"))
            {
                throw new Exception("Not authenticated");
            }

            var header = Request.Headers["x-drawwars-auth"];
            var userId = CryptoUtils.GetUserIdFromHeader(header);

            return _drawingRepository
                .ListByUser(userId)
                .Select(d => new
                {
                    d.GameRoomId,
                    d.PlayerId,
                    d.ThemeId,
                    _themeRepository.Get(d.ThemeId).Text,
                    d.Url
                })
                .ToList();
        }
    }
}
