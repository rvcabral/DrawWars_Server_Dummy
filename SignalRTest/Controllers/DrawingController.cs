using DrawWars.Aws;
using Microsoft.AspNetCore.Mvc;
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
            //TODO Validate model.SessionID and model.PlayerID as 

            var filename = $"{model.SessionID.ToString("N")}{model.PlayerID.ToString("N")}.{model.Extension}";
            var picture = Convert.FromBase64String(model.Drawing);

            var uri = new AwsManager().S3_UploadFile(filename, picture);

            return new { uri };
        }
    }
}
