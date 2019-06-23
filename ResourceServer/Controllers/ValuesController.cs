using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Security.OAuth.Introspection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ResourceServer.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthIntrospectionDefaults.AuthenticationScheme)]
    public class ValuesController : ControllerBase
    {
        // GET /api/values/Get
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = User?.Identity?.Name;
                var email = User?.FindFirst(ClaimTypes.Email)?.Value;
                if (userId == null || email == null)
                {
                    return Forbid();
                }

                await Task.Delay(10);
                var result = new { userId = userId, email = email };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
