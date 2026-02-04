namespace TimeForMoney.Api.Models;

public class Contact {
    public int Id { get; set; }
    public int ClientId { get; set; }
    public Client? Client { get; set; }
    public ContactType Type { get; set; } = ContactType.Email;
    public string Value { get; set; } = string.Empty;
    public string? Note { get; set; }
}
