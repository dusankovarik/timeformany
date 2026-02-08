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

    // GET: api/session-payments/session/1/balance
    [HttpGet("session/{sessionId}/balance")]
    public async Task<ActionResult<SessionBalanceDto>> GetSessionBalance(
        [FromRoute] int sessionId) {
        var result = await _service.GetSessionBalanceAsync(sessionId);

        if (result == null) {
            return NotFound($"Session with ID {sessionId} does not exist.");
        }

        return Ok(result);
    }

    // GET: api/session-payments/1
    [HttpGet("{id}")]
    public async Task<ActionResult<AssignmentDto>> GetAssignment([FromRoute] int id) {
        var result = await _service.GetAssignmentAsync(id);

        if (result == null) {
            return NotFound($"Assignment with ID {id} does not exist.");
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

    // PUT: api/session-payments/1
    [HttpPut("{id}")]
    public async Task<ActionResult<EditAssignmentResponseDto>> EditAssignment(
        [FromRoute] int id,
        [FromBody] EditAssignmentRequestDto request) {
        var result = await _service.EditAssignmentAsync(id, request);

        if (!result.Success) {
            return BadRequest(result);
        }

        return Ok(result);
    }

    // DELETE: api/session-payments/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAssignment([FromRoute] int id) {
        var deleted = await _service.DeleteAssignmentAsync(id);

        if (!deleted) {
            return NotFound($"Assignment with ID {id} does not exist.");
        }

        return NoContent();
    }
}
