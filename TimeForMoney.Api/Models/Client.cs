namespace TimeForMoney.Api.Models;
public class Client {
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public decimal HourlyRate { get; set; }
    public DateOnly? StartDate { get; set; }
    public string? AcquisitionSource { get; set; }
    public ClientStatus Status { get; set; } = ClientStatus.Active;
    public string? Note { get; set; }
}
