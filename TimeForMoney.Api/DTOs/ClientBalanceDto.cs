public class ClientBalanceDto {
    public int ClientId { get; set; }
    public string ClientFullName { get; set; } = string.Empty;
    public decimal TotalSessionsPrice { get; set; }
    public decimal TotalPaidAmount { get; set; }
    public decimal Balance { get; set; }
    public int TotalSessionsCount { get; set; }
    public int PaidSessionsCount { get; set; }
    public int UnpaidSessionsCount { get; set; }
}
