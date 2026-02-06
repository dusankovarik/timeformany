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

    public async Task<ClientBalanceDto?> GetClientBalanceAsync(int clientId) {
        // VALIDATE: Check if client exists
        var client = await _context.Clients.FindAsync(clientId);
        if (client == null) {
            return null;
        }
        
        // ACTION: Read all sessions for the client
        var sessions = await _context.Sessions
            .Where(s => s.ClientId == clientId)
            .ToListAsync();
        
        // ACTION: Calculate total sessions price
        var totalSessionsPrice = sessions.Sum(s => s.HourlyRate * s.Duration + s.TravelFee + s.Adjustment);
        
        // ACTION: Calculate total paid amount (sum of all payments from this client)
        var totalPaidAmount = await _context.Payments
            .Where(p => p.ClientId == clientId)
            .SumAsync(p => p.Amount);
        
        // ACTION: Calculate remaining balance (positive = credit, negative = debt)
        var balance = totalPaidAmount - totalSessionsPrice;

        // ACTION: Calculate, how many sessions are paid and how many are unpaid
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

        // RESPONSE: Create and return DTO
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
}
