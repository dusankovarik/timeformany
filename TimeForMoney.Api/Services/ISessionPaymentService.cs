namespace TimeForMoney.Api.Services;

using TimeForMoney.Api.DTOs;

public interface ISessionPaymentService {
    /// <summary>
    /// Find the payment status of a session - how much is paid, how much remains.
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
}
