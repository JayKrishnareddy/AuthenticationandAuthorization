using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AuthenticationandAuthorization.Controllers
{
    public class AuthenticateController : BaseController
    {
        #region Property
        /// <summary>
        /// Property Declaration
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private IConfiguration _config;

        #endregion

        #region Contructor Injector
        /// <summary>
        /// Constructor Injection to access all methods or simply DI(Dependency Injection)
        /// </summary>
        public AuthenticateController(IConfiguration config)
        {
            _config = config;
        }
        #endregion

        #region GenerateJWT
        /// <summary>
        /// Generate Json Web Token Method
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        private string GenerateJSONWebToken(LoginModel userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              null,
              expires: DateTime.Now.AddMinutes(120),
              signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        #endregion

        #region AuthenticateUser
        /// <summary>
        /// Hardcoded the User authentication
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private async Task<LoginModel> AuthenticateUser(LoginModel login)
        {
            LoginModel user = null;

            //Validate the User Credentials    
            //Demo Purpose, I have Passed HardCoded User Information    
            if (login.UserName == "Jay")
            {
                user = new LoginModel { UserName = "Jay", Password = "123456" };
            }
            return user;
        }
        #endregion

        #region Login Validation
        /// <summary>
        /// Login Authenticaton using JWT Token Authentication
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost(nameof(Login))]
        public async Task<IActionResult> Login([FromBody] LoginModel data)
        {
            IActionResult response = Unauthorized();
            var user = await AuthenticateUser(data);
            if (data != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new { Token = tokenString , Message = "Success" });
            }
            return response;
        }
        #endregion

        #region Get
        /// <summary>
        /// Authorize the Method
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(Get))]
        public async Task<IEnumerable<string>> Get()
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            return new string[] { accessToken };
        }


        #endregion

    }

    #region JsonProperties
    /// <summary>
    /// Json Properties
    /// </summary>
    public class LoginModel
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }
    }
    #endregion
}
