namespace TimeForMoney.Api.Data;

using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Models;

public class AppDbContext : DbContext {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<Payment> Payments { get; set; } = null!;
    public DbSet<SessionPayment> SessionPayments { get; set; } = null!;
}
