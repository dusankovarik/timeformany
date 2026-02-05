namespace TimeForMoney.Api.DTOs;

public class AssignPaymentResponseDto {
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public decimal TotalAssignedAmount { get; set; }
    public decimal RemainingPaymentAmount { get; set; }
    public int AssignedSessionsCount { get; set; }
}
