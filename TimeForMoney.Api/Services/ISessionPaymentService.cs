namespace TimeForMoney.Api.Services;

using TimeForMoney.Api.DTOs;

public interface ISessionPaymentService {
    /// <summary>
    /// Get the balance of a session - how much is paid, how much remains.
    /// </summary>
    /// <param name="sessionId">Session ID.</param>
    /// <returns>Balance for the session or null if the session does not exist.</returns>
    Task<SessionBalanceDto?> GetSessionBalanceAsync(int sessionId);

    /// <summary>
    /// Get details of a specific session payment assignment.
    /// </summary>
    /// <param name="assignmentId">Assignment ID.</param>
    /// <returns>Assignment details or null if assignment does not exist.</returns>
    Task<AssignmentDto?> GetAssignmentAsync(int assignmentId);

    /// <summary>
    /// Assign a payment to one or more sessions.
    /// Validate that payment and sessions exist, belong to the same client,
    /// and that the total amount does not exceed the remaining payment amount.
    /// </summary>
    /// <param name="request">Request with paymentId and list of assignments.</param>
    /// <returns>Response with operation result.</returns>
    Task<AssignPaymentResponseDto> AssignPaymentToSessionsAsync(AssignPaymentRequestDto request);

    /// <summary>
    /// Edit an existing session payment assignment by changing the assigned amount.
    /// Validate that the new amount does not exceed the remaining payment amount.
    /// </summary>
    /// <param name="assignmentId">Assignment ID.</param>
    /// <param name="request">Request with new amount.</param>
    /// <returns>Response with operation result.</returns>
    Task<EditAssignmentResponseDto> EditAssignmentAsync(int assignmentId, EditAssignmentRequestDto request);

    /// <summary>
    /// Delete a session payment assignment.
    /// The payment amount becomes available for other assignments.
    /// </summary>
    /// <param name="assignmentId">Assignment ID.</param>
    /// <returns>True if deleted successfully, false if assignment does not exist.</returns>
    Task<bool> DeleteAssignmentAsync(int assignmentId);
}
