using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;

namespace HealthCheckAPI.HealthCheck;

public class ICMPHealthCheck : IHealthCheck
{
  private readonly string Host = $"10.0.0.0";
  private readonly int HealthyRoundtripTime = 300;

  async Task<HealthCheckResult> IHealthCheck.CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken)
  {
		try
		{
			using var ping = new Ping();
			var reply = await ping.SendPingAsync(Host);

			switch(reply.Status)
			{
				case IPStatus.Success:
					return (reply.RoundtripTime > HealthyRoundtripTime) ? HealthCheckResult.Degraded() : HealthCheckResult.Healthy();
				default:
					return HealthCheckResult.Unhealthy();
			}
		}
		catch (Exception ex)
		{
			return HealthCheckResult.Unhealthy();
		}
  }
}

