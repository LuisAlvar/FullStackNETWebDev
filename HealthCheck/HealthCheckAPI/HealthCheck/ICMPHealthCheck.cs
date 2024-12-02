using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;

namespace HealthCheckAPI.HealthCheck;

public class ICMPHealthCheck : IHealthCheck
{
  private readonly string Host = $"10.0.0.0";
  private readonly int HealthyRoundtripTime = 300;

	public ICMPHealthCheck(string host, int healthyRoundtripTime)
  {
    Host = host;
    HealthyRoundtripTime = healthyRoundtripTime;
  }

  async Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
  {
		try
		{
			using Ping ping = new Ping();
			var reply = await ping.SendPingAsync(Host);

			switch(reply.Status)
			{
				case IPStatus.Success:
					string msg = $"ICMP to {Host} took {reply.RoundtripTime} ms";
					return (reply.RoundtripTime > HealthyRoundtripTime) ? HealthCheckResult.Degraded(msg) : HealthCheckResult.Healthy(msg);
				default:
					string err = $"ICMP to {Host} failed: {reply.Status}";
					return HealthCheckResult.Unhealthy(err);
			}
		}
		catch (Exception ex)
		{
      string err = $"ICMP to {Host} failed: {ex.Message.ToString()}";
      return HealthCheckResult.Unhealthy(err);
		}
  }
}

