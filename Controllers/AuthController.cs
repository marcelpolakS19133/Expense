using Expense.Authentication;
using Expense.DTO;
using Expense.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Expense.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly JwtIssuerOptions _jwtOptions;
        private readonly JwtFactory _jwtFactory;
        private readonly UserManager<AppUser> _userManager;
        public AuthController(UserManager<AppUser> userManager, IOptions<JwtIssuerOptions> jwtOptions, JwtFactory jwtFactory)
        {
            _jwtOptions = jwtOptions.Value;
            _jwtFactory = jwtFactory;
            _userManager = userManager;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(CredentialsDTO credentials)
        {
            var result = await _userManager.CreateAsync(new AppUser(credentials.Username, "aaa@basdasd.pl"), credentials.Password);
            return new OkObjectResult(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(CredentialsDTO credentials)
        {
            var identity = await GetClaimsIdentity(credentials.Username, credentials.Password);

            var jwt = await GenerateJwt(identity, _jwtFactory, credentials.Username, _jwtOptions);
            Response.Cookies.Append("jwt", jwt.AuthToken, new CookieOptions { HttpOnly = true, MaxAge = TimeSpan.FromSeconds(jwt.ExpiresIn) });
            Response.Cookies.Append("currentUser", jwt.Id, new CookieOptions { MaxAge = TimeSpan.FromSeconds(jwt.ExpiresIn) });
            return new OkResult();
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            var userToVerify = await _userManager.FindByNameAsync(username);

            if (await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(username, userToVerify.Id));
            }



            return await Task.FromResult<ClaimsIdentity>(null);
        }

        public static async Task<JWT> GenerateJwt(ClaimsIdentity identity, JwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions)
        {
            var response = new JWT
            {
                Id = identity.Claims.Single(c => c.Type == "id").Value,
                AuthToken = await jwtFactory.GenerateEncodedToken(userName, identity),
                ExpiresIn = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return response;
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("jwt");
            Response.Cookies.Delete("currentUser");
            return new OkResult();
        }
    }
}
