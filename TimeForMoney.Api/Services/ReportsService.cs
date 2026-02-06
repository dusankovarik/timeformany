namespace TimeForMoney.Api.Services;

using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.DTOs;

public class ReportsService : IReportsService {
    private readonly AppDbContext _context;

    public ReportsService(AppDbContext context) {
        _context = context;
    }

    public async Task<ClientBalanceDto?> GetClientBalanceAsync(int clientId) {
        // Check if client exists
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null) {
            return null;
        }
        
        // Load all client's sessions
        var sessions = await _context.Sessions
            .Where(s => s.ClientId == clientId)
            .ToListAsync();
        
        // Calculate total sessions price
        var totalSessionsPrice = sessions.Sum(s => s.HourlyRate * s.Duration + s.TravelFee + s.Adjustment);
        
        // Calculate total paid amount (sum of all payments from client)
        var totalPaidAmount = await _context.Payments
            .Where(p => p.ClientId == clientId)
            .SumAsync(p => p.Amount);
        
        // Calculate balance (positive = credit, negative = debt)
        var balance = totalPaidAmount - totalSessionsPrice;

        // Count paid vs unpaid sessions
        var paidSessionsCount = 0;
        var unpaidSessionsCount = 0;

        foreach (var session in sessions) {
            var sessionPrice = session.HourlyRate * session.Duration
                + session.TravelFee + session.Adjustment;

            var paidAmount = await _context.SessionPayments
                .Where(sp => sp.SessionId == session.Id)
                .SumAsync(sp => sp.Amount);

            if (paidAmount >= sessionPrice) {
                paidSessionsCount++;
            } else {
                unpaidSessionsCount++;
            }
        }

        // Return DTO
        return new ClientBalanceDto {
            ClientId = clientId,
            ClientFullName = $"{client.FirstName} {client.LastName}",
            TotalSessionsPrice = totalSessionsPrice,
            TotalPaidAmount = totalPaidAmount,
            Balance = balance,
            TotalSessionsCount = sessions.Count,
            PaidSessionsCount = paidSessionsCount,
            UnpaidSessionsCount = unpaidSessionsCount
        };
    }

    public async Task<IncomeBySessionsDto> GetIncomeBySessionsAsync(DateOnly from, DateOnly to) {
        // Load all sessions in the period
        var sessions = await _context.Sessions
            .Where(s => s.Date >= from && s.Date <= to)
            .ToListAsync();

        // Calculate total income (all sessions regardless of payment status)
        var totalIncome = sessions.Sum(s => s.HourlyRate * s.Duration + s.TravelFee + s.Adjustment);

        // Calculate paid and unpaid amounts
        var paidIncome = 0m;
        var unpaidIncome = 0m;

        foreach (var session in sessions) {
            var sessionPrice = session.HourlyRate * session.Duration
                + session.TravelFee + session.Adjustment;

            var paidAmount = await _context.SessionPayments
                .Where(sp => sp.SessionId == session.Id)
                .SumAsync(sp => sp.Amount);

            paidIncome += paidAmount;
            unpaidIncome += sessionPrice - paidAmount;
        }

        // Return DTO
        return new IncomeBySessionsDto {
            PeriodFrom = from,
            PeriodTo = to,
            TotalSessionsCount = sessions.Count,
            TotalIncome = totalIncome,
            PaidIncome = paidIncome,
            UnpaidIncome = unpaidIncome
        };
    }

    public async Task<IncomeByPaymentsDto> GetIncomeByPaymentsAsync(DateOnly from, DateOnly to) {
        // Load all payments in the period
        var payments = await _context.Payments
            .Where(p => p.Date >= from && p.Date <= to)
            .ToListAsync();

        // Calculate total income (sum of all payment amounts)
        var totalIncome = payments.Sum(p => p.Amount);

        // Return DTO
        return new IncomeByPaymentsDto {
            PeriodFrom = from,
            PeriodTo = to,
            TotalPaymentsCount = payments.Count,
            TotalIncome = totalIncome
        };
    }
}
