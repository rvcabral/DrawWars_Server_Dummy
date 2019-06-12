using DrawWars.Aws;
using Microsoft.AspNetCore.Mvc;
using SignalRTest.GameManager;
using SignalRTest.Logic;
using SignalRTest.Models;
using System;

namespace SignalRTest.Controllers
{
    [ApiController]
    [Route("api/drawing")]
    public class DrawingController : Controller
    {
        [HttpPost("submit")]
        public object Submit([FromBody] PictureUploadModel model)
        {
            
            var context = new Context(model.SessionID, model.PlayerID);
            if (CoreManager.ValidContext(context))
            {
                var filename = $"{model.SessionID.ToString("N")}{model.PlayerID.ToString("N")}{Guid.NewGuid()}.{model.Extension}";
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

        [HttpGet("time")]
        public DateTimeOffset GetTime() => DateTimeOffset.UtcNow;


        [HttpGet("HowManySessionsAtPlay")]
        public int GetSessionsCount() => CoreManager.GetSessionCount();
    }
}
