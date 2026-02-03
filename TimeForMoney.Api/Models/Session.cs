namespace TimeForMoney.Api.Models;

public class Session {
    public int Id { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public decimal Duration { get; set; }
    public decimal HourlyRate { get; set; }
    public string? Topic { get; set; }
    public string? Note { get; set; }
}
