namespace TimeForMoney.Api.Models;

public class SessionPayment {
    public int Id { get; set; }
    public int SessionId { get; set; }
    public Session? Session { get; set; }
    public int PaymentId { get; set; }
    public Payment? Payment { get; set; }
    public decimal Amount { get; set; }
}
