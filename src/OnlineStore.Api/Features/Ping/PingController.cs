using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Api.Features.Ping;

/// <summary>
/// Найпростіший ендпоінт для перевірки, що API запущене.
/// </summary>
[ApiController]
[Route("api/ping")]
public class PingController(IPingService ping) : ControllerBase
{
    /// <summary>
    /// GET /api/ping — повертає "pong" і поточний час UTC.
    /// </summary>
    [HttpGet]
    public ActionResult<PingResponse> Get() => Ok(ping.Get());
}
