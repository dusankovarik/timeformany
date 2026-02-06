namespace TimeForMoney.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using TimeForMoney.Api.DTOs;
using TimeForMoney.Api.Services;

[ApiController]
[Route("api/reports")]
public class ReportsController : ControllerBase {
    private readonly IReportsService _service;

    public ReportsController(IReportsService service) {
        _service = service;
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

    // GET: api/reports/income-by-sessions?from=2025-01-01&to=2025-01-31
    [HttpGet("income-by-sessions")]
    public async Task<ActionResult<IncomeBySessionsDto>> GetIncomeBySessions(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to) {
        var result = await _service.GetIncomeBySessionsAsync(from, to);
        return Ok(result);
    }

    // GET: api/reports/income-by-payments?from=2025-01-01&to=2025-01-31
    [HttpGet("income-by-payments")]
    public async Task<ActionResult<IncomeByPaymentsDto>> GetIncomeByPayments(
        [FromQuery] DateOnly from,
        [FromQuery] DateOnly to) {
        var result = await _service.GetIncomeByPaymentsAsync(from, to);
        return Ok(result);
    }
}
