namespace TimeForMoney.Api.Services;

using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.DTOs;

public class SessionPaymentService : ISessionPaymentService {
    private readonly AppDbContext _context;

    public SessionPaymentService(AppDbContext context) {
        _context = context;
    }

    public async Task<SessionPaymentStatusDto?> GetSessionPaymentStatusAsync(int sessionId) {
        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null) {
            return null;
        }

        var sessionPrice = (session.HourlyRate * session.Duration)
            + session.TravelFee
            + session.Adjustment;

        var paidAmount = await _context.SessionPayments
            .Where(sp => sp.SessionId == sessionId)
            .SumAsync(sp => sp.Amount);

        var remainingAmount = sessionPrice - paidAmount;

        return new SessionPaymentStatusDto {
            SessionId = sessionId,
            SessionPrice = sessionPrice,
            PaidAmount = paidAmount,
            RemainingAmount = remainingAmount,
            IsPaid = remainingAmount <= 0,
        };
    }
}
