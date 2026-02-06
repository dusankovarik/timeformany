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
    public async Task<ActionResult<SessionPaymentStatusDto>> GetSessionPaymentStatus(
        [FromRoute] int sessionId) {
        var result = await _service.GetSessionPaymentStatusAsync(sessionId);

        if (result == null) {
            return NotFound($"Session with ID {sessionId} was not found.");
        }

        return Ok(result);
    }

    // POST: api/session-payments/assign
    [HttpPost("assign")]
    public async Task<ActionResult<AssignPaymentResponseDto>> AssignPaymentToSessions(
        [FromBody] AssignPaymentRequestDto request) {
        var result = await _service.AssignPaymentToSessionsAsync(request);

        if (!result.Success) {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // GET: api/session-payments/client/1/balance
    [HttpGet("client/{clientId}/balance")]
    public async Task<ActionResult<ClientBalanceDto>> GetClientBalance(
        [FromRoute] int clientId) {
        var result = await _service.GetClientBalanceAsync(clientId);

        if (result == null) {
            return NotFound($"Client with ID {clientId} was not found.");
        }

        return Ok(result);
    }
}
