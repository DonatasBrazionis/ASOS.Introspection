using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers
{
    [Authorize]
    public class ValuesController : ControllerBase
    {
        // GET /api/values/Get
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var userId = User?.Identity?.Name;
                if (userId == null)
                {
                    return Forbid();
                }

                await Task.Delay(10);
                var result = new { UserId = userId };
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
