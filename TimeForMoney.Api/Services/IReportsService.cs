namespace TimeForMoney.Api.Services;

using TimeForMoney.Api.DTOs;

public interface IReportsService {
    /// <summary>
    /// Get the balance for a given client - total sessions price, total paid, and remaining balance.
    /// Positive balance means credit (prepaid), negative means debt (unpaid).
    /// </summary>
    /// <param name="clientId">Client ID.</param>
    /// <returns>Client balance or null if client does not exist.</returns>
    Task<ClientBalanceDto?> GetClientBalanceAsync(int clientId);

    /// <summary>
    /// Get income report based on sessions that occured within the specified period.
    /// Show total income from sessions regardless of payment status, and break down
    /// how much is pais vs unpaid (accural basis).
    /// </summary>
    /// <param name="from">Start date of the period (inclusive).</param>
    /// <param name="to">End date of the period (inclusive).</param>
    /// <returns>Income report by sessions.</returns>
    Task<IncomeBySessionsDto> GetIncomeBySessionsAsync(DateOnly from, DateOnly to);

    /// <summary>
    /// Get income report based on payments received within the specified period.
    /// Show actual cash received (cash basis).
    /// </summary>
    /// <param name="from">Start date of the period (inclusive).</param>
    /// <param name="to">End date of the period (inclusive).</param>
    /// <returns>Income report by payments.</returns>
    Task<IncomeByPaymentsDto> GetIncomeByPaymentsAsync(DateOnly from, DateOnly to);
}
