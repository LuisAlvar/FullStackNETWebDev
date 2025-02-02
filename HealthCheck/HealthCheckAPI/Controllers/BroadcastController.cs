using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace HealthCheckAPI.Controllers
{
  [Route("api/[controller]/[action]")]
  [ApiController]
  public class BroadcastController : ControllerBase
  {
    private IHubContext<HealthCheckHub> _hub;

    public BroadcastController(IHubContext<HealthCheckHub> hub)
    {
      _hub = hub;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> Update()
    {
      await _hub.Clients.All.SendAsync("Update", "test");
      return Ok("Update message sent.");
    }

  }
}
