using Expense.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Expense.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FBAuthController : ControllerBase
    {
        private readonly IAuthService authService;

        public FBAuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        [HttpGet("step1")]
        public ActionResult Auth(string code)
        {   
            return Ok(authService.DoAuth(code));
        }
        [HttpGet("step2")]
        public ActionResult Auth2()
        {
            return Ok();
        }

    }
}
