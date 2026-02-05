namespace TimeForMoney.Api.DTOs;

public class AssignPaymentRequestDto {
    public int PaymentId { get; set; }
    public List<SessionAssignmentDto> Assignments { get; set; } = new();
}
