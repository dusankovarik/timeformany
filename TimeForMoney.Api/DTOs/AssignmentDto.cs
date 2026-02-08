namespace TimeForMoney.Api.DTOs;

public class AssignmentDto {
    public int Id { get; set; }
    public int SessionId { get; set; }
    public int PaymentId { get; set; }
    public decimal Amount { get; set; }
}
