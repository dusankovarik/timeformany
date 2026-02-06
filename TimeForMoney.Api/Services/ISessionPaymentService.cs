namespace TimeForMoney.Api.Services;

using TimeForMoney.Api.DTOs;

public interface ISessionPaymentService {
    /// <summary>
    /// Find the payment status for a given session.
    /// </summary>
    /// <param name="sessionId">Session ID.</param>
    /// <returns>Payment status for the session or null if the session does not exist.</returns>
    Task<SessionPaymentStatusDto?> GetSessionPaymentStatusAsync(int sessionId);

    /// <summary>
    /// Assign a payment to one or more sessions.
    /// Validate that payment and sessions exist, belong to the same client,
    /// and that the total amount does not exceed the remaining payment amount.
    /// </summary>
    /// <param name="request">Request with paymentId and list of assignments.</param>
    /// <returns>Response with operation result.</returns>
    Task<AssignPaymentResponseDto> AssignPaymentToSessionsAsync(AssignPaymentRequestDto request);

    /// <summary>
    /// Get the balance for a given client - total sessions price, total paid, and remaining balance.
    /// Positive balance means credit (prepaid), negative means debt (unpaid).
    /// </summary>
    /// <param name="clientId">Client ID.</param>
    /// <returns>Client balance or null if client does not exist.</returns>
    Task<ClientBalanceDto?> GetClientBalanceAsync(int clientId);
}
