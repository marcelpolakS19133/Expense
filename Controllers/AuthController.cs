using Expense.Authentication;
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
    [Route("api/[controller]")]
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
        public async Task<IActionResult> Register(string username, string password)
        {
            var result = await _userManager.CreateAsync(new AppUser(username, "aaa@basdasd.pl"), password);
            return new OkObjectResult(result);
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login(string username, string password)
        {
            var identity = await GetClaimsIdentity(username, password);

            var jwt = await GenerateJwt(identity, _jwtFactory, username, _jwtOptions);
            return new OkObjectResult(jwt);
        }

        private async Task<ClaimsIdentity> GetClaimsIdentity(string username, string password)
        {
            var userToVerify = await _userManager.FindByNameAsync(username);

            if(await _userManager.CheckPasswordAsync(userToVerify, password))
            {
                return await Task.FromResult(_jwtFactory.GenerateClaimsIdentity(username, userToVerify.Id));
            }
            return await Task.FromResult<ClaimsIdentity>(null);
        }

        public static async Task<object> GenerateJwt(ClaimsIdentity identity, JwtFactory jwtFactory, string userName, JwtIssuerOptions jwtOptions)
        {
            var response = new
            {
                id = identity.Claims.Single(c => c.Type == "id").Value,
                auth_token = await jwtFactory.GenerateEncodedToken(userName, identity),
                expires_in = (int)jwtOptions.ValidFor.TotalSeconds
            };

            return response;
        }
    }
}
