using DrawWars.Api.GameManager;
using DrawWars.Api.Logic;
using DrawWars.Api.Models;
using DrawWars.Aws;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace DrawWars.Api.Controllers
{
    [ApiController]
    [Route("api/drawing")]
    public class DrawingController : Controller
    {
        private IDrawingRepository _drawingRepository;

        public DrawingController(IDrawingRepository drawingRepo) => 
            _drawingRepository = drawingRepo;

        [HttpPost("submit")]
        public object Submit([FromBody] PictureUploadModel model)
        {
            
            var context = new Context(model.SessionID, model.PlayerID);
            if (CoreManager.ValidContext(context))
            {
                var filename = $"{model.SessionID.ToString("N")}{model.PlayerID.ToString("N")}{Guid.NewGuid().ToString("N")}.{model.Extension}";
                var picture = Convert.FromBase64String(model.Drawing);
                try
                {
                    var uri = new AwsManager().S3_UploadFile(filename, picture);

                    CoreManager.setDraw(context, uri, model.Theme);

                    return new { uri, errorMessage = string.Empty };
                }
                catch (Exception)
                {
                    return new { uri = string.Empty, errorMessage = "Unable to upload draw" };
                }
            }
            return new {  uri = string.Empty, errorMessage = "Invalid Context" };
        }
    }
}
