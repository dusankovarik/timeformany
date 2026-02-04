namespace TimeForMoney.Api.Models;

public class Payment {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public decimal Amount { get; set; }
    public PaymentMethod Method { get; set; } = PaymentMethod.Cash;
    public string? Note { get; set; }
}
