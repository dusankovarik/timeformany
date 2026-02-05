namespace TimeForMoney.Api.Services;

using Microsoft.EntityFrameworkCore;
using TimeForMoney.Api.Data;
using TimeForMoney.Api.DTOs;
using TimeForMoney.Api.Models;

public class SessionPaymentService : ISessionPaymentService {
    private readonly AppDbContext _context;

    public SessionPaymentService(AppDbContext context) {
        _context = context;
    }

    public async Task<SessionPaymentStatusDto?> GetSessionPaymentStatusAsync(int sessionId) {
        // VALIDATE: Check if session exists
        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null) {
            return null;
        }

        // ACTION: Calculate session price
        var sessionPrice = (session.HourlyRate * session.Duration)
            + session.TravelFee
            + session.Adjustment;

        // ACTION: Calculate paid amount
        var paidAmount = await _context.SessionPayments
            .Where(sp => sp.SessionId == sessionId)
            .SumAsync(sp => sp.Amount);

        // ACTION: Calculate remaining amount
        var remainingAmount = sessionPrice - paidAmount;

        // RESPONSE: Create and return DTO
        return new SessionPaymentStatusDto {
            SessionId = sessionId,
            SessionPrice = sessionPrice,
            PaidAmount = paidAmount,
            RemainingAmount = remainingAmount,
            IsPaid = remainingAmount <= 0,
        };
    }

    public async Task<AssignPaymentResponseDto> AssignPaymentToSessionsAsync(
        AssignPaymentRequestDto request) {
        // VALIDATE: Check if payment exists
        var payment = await _context.Payments.FindAsync(request.PaymentId);
        if (payment == null) {
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Payment with ID {request.PaymentId} does not exist."
            };
        }

        // VALIDATE: Check if there are any assignments
        if (!request.Assignments.Any()) {
            return new AssignPaymentResponseDto {
                Success = false,
                Message = "No session assignments provided."
            };
        }

        // VALIDATE: Check if all sessions exist
        var sessionIds = request.Assignments.Select(a => a.SessionId).ToList();
        var sessions = await _context.Sessions
            .Where(s => sessionIds.Contains(s.Id))
            .ToListAsync();

        if (sessions.Count != sessionIds.Count) {
            var missingSessions = sessionIds.Except(sessions.Select(s => s.Id));
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Sessions with IDs {string.Join(", ", missingSessions)} do not exist."
            };
        }

        // VALIDATE: Check if all sessions belong to the same client as the payment
        if (sessions.Any(s => s.ClientId != payment.ClientId)) {
            var wrongSessions = sessions.Where(s => s.ClientId != payment.ClientId)
                                    .Select(s => s.Id);
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Sessions with IDs {string.Join(", ", wrongSessions)} do not belong "
                    + $"to the same client as the payment with ID {payment.Id}."
            };
        }

        // VALIDATE: Calculate the already assigned amount from this payment
        var alreadyAssigned = await _context.SessionPayments
            .Where(sp => sp.PaymentId == request.PaymentId)
            .SumAsync(sp => sp.Amount);

        // VALIDATE: Calculate the new assigned amount
        var newAssignments = request.Assignments.Sum(a => a.Amount);

        // VALIDATE: Check if we exceed the payment amount
        var totalAssigned = alreadyAssigned + newAssignments;
        if (totalAssigned > payment.Amount) {
            var remaining = payment.Amount - alreadyAssigned;
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Cannot assign {newAssignments} CZK. Payment has {payment.Amount} CZK, " +
                        $"already assigned {alreadyAssigned} CZK, only {remaining} CZK remaining."
            };
        }

        // ACTION: Create SessionPayment records
        foreach (var assignment in request.Assignments) {
            var sessionPayment = new SessionPayment {
                SessionId = assignment.SessionId,
                PaymentId = request.PaymentId,
                Amount = assignment.Amount
            };
            _context.SessionPayments.Add(sessionPayment);
        }

        // ACTION: Save to database
        await _context.SaveChangesAsync();

        // RESPONSE: Return successful response
        return new AssignPaymentResponseDto {
            Success = true,
            Message = $"Payment successfully assigned to {request.Assignments.Count} session(s).",
            TotalAssignedAmount = newAssignments,
            RemainingPaymentAmount = payment.Amount - totalAssigned,
            AssignedSessionsCount = request.Assignments.Count
        };
    }
}
