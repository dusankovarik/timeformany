namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TimeForMoney.Api.DTOs;
using TimeForMoney.Api.Services;

[ApiController]
[Route("api/session-payments")]
public class SessionPaymentsController : ControllerBase {
    private readonly ISessionPaymentService _service;

    public SessionPaymentsController(ISessionPaymentService service) {
        _service = service;
    }

    // GET: api/session-payments/session/1/status
    [HttpGet("session/{sessionId}/status")]
    public async Task<ActionResult<SessionPaymentStatusDto>> GetSessionPaymentStatus(int sessionId) {
        var result = await _service.GetSessionPaymentStatusAsync(sessionId);

        if (result == null) {
            return NotFound($"Session with ID {sessionId} was not found.");
        }

        return Ok(result);
    }
}
