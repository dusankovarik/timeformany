namespace TimeForMoney.Api.DTOs;

public class SessionBalanceDto {
    public int SessionId { get; set; }
    public decimal SessionPrice { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal RemainingAmount { get; set; }
    public bool IsPaid { get; set; }
}
