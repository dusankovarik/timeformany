namespace TimeForMoney.Api.DTOs;

public class EditAssignmentResponseDto {
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int AssignmentId { get; set; }
    public decimal OldAmount { get; set; }
    public decimal NewAmount { get; set; }
    public decimal RemainingPaymentAmount { get; set; }
}
