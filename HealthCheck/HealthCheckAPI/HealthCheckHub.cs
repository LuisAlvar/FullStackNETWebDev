using Microsoft.AspNetCore.SignalR;

namespace HealthCheckAPI
{
 
  public class HealthCheckHub: Hub
  {
    /*
     * The class is intentially empty since we dont need to add any method yet:
     * hwoever, the importatnt thign was to have it derived from the Hub base class,
     * which is a requirement for any SignalR hub.
     */

    /// <summary>
    /// This allows for bi-directional communication via SignalR
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public async Task ClientUpdate(string message) => await Clients.All.SendAsync("ClientUpdate", message);
  }
}
