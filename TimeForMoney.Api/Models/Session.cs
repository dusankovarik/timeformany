namespace TimeForMoney.Api.Models;

public class Session {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public decimal Duration { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public SessionFormat Format { get; set; } = SessionFormat.Online;
    public decimal HourlyRate { get; set; }
    public decimal TravelFee { get; set; } = 0;
    public decimal Adjustment { get; set; } = 0;
    public string? Topic { get; set; }
    public string? Note { get; set; }
}
