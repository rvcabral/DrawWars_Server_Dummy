﻿using DrawWars.Aws;
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
        [RequestSizeLimit(100_000_000)]
        public object Submit([FromBody] PictureUploadModel model)
        {
            
            var context = new Context(model.SessionID, model.PlayerID);
            if (CoreManager.ValidContext(context))
            {
                var filename = $"{model.SessionID.ToString("N")}{model.PlayerID.ToString("N")}.{model.Extension}";
                var picture = Convert.FromBase64String(model.Drawing);
                try
                {
                    var uri = new AwsManager().S3_UploadFile(filename, picture);
                    CoreManager.setDraw(context, uri, model.Theme);
                    return new { uri, errorMessage = string.Empty };
                }
                catch (Exception e)
                {
                    return new { uri = string.Empty, errorMessage = "crap." };
                }
            }
            return new { uri = string.Empty, errorMessage = "Invalid Context" };
        }

    }
}
