using DrawWars.Aws;
using Microsoft.AspNetCore.Mvc;
using SignalRTest.Models;
using System;

namespace SignalRTest.Controllers
{
    [Route("api/drawing")]
    public class DrawingController : Controller
    {
        [HttpPost]
        public string Submit([FromBody] PictureUploadModel model)
        {
            //TODO Validate model.SessionID and model.PlayerID as 

            var filename = $"{model.SessionID}{model.PlayerID}.{model.Extension}";
            var picture = Convert.FromBase64String(model.Drawing);

            var uri = new AwsManager().S3_UploadFile(filename, picture);

            return uri;
        }
    }
}
