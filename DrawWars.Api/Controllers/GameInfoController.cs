using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrawWars.Api.GameManager;
using DrawWars.Api.Logic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DrawWars.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameInfoController : ControllerBase
    {
        [HttpGet("interactionCount/{session}/{player}")]
        public object GetInteractionCount(Guid session, Guid player)
        {
            var count = CoreManager.GetPlayerInteractionCount(session, player);
            return count;
        }
    }
}