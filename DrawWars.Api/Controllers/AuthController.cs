using DrawWars.Api.Models;
using DrawWars.Api.Utils;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DrawWars.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private IDrawWarsUserRepository _userRepository;

        private IConfiguration _configuration;

        public AuthController(IDrawWarsUserRepository userRepo, IConfiguration config)
        {
            _userRepository = userRepo;
            _configuration = config;
        }

        [HttpPost]
        [Route("register")]
        public object Register([FromBody]LoginModel input)
        {
            if( string.IsNullOrWhiteSpace(input.Email) 
            ||  string.IsNullOrWhiteSpace(input.Password) 
            ||  _userRepository.GetByUsername(input.Email) != null
            )
            {
                throw new Exception("User already exists.");
            }

            var newUser = new DrawWarsUser()
            {
                Username = input.Email,
                PassHash = CryptoUtils.HashPassword(input.Password)
            };

            var user = _userRepository.Create(newUser);

            SetAuthHeader(user);

            return new { Email = user.Username, user.Id };
        }

        [HttpPost]
        [Route("login")]
        public object Login([FromBody]LoginModel input)
        {
            if (string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
                throw new Exception("Invalid credentials.");

            var user = _userRepository.GetByUsername(input.Email);
            if(user == null)
                throw new Exception("User not found.");

            if(!CryptoUtils.ComparePassword(input.Password, user.PassHash))
                throw new Exception("Incorrect password.");

            SetAuthHeader(user);
            
            return new { Email = user.Username, user.Id };
        }

        #region Private Utis

        private void SetAuthHeader(DrawWarsUser user)
        {
            Response.Headers.Add("x-drawwars-auth", CryptoUtils.CypherHeader(user));
        }

        #endregion
    }
}
