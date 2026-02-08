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

    public async Task<SessionBalanceDto?> GetSessionBalanceAsync(int sessionId) {
        // Check if session exists
        var session = await _context.Sessions.FindAsync(sessionId);
        if (session == null) {
            return null;
        }

        // Calculate session price
        var sessionPrice = (session.HourlyRate * session.Duration)
            + session.TravelFee
            + session.Adjustment;

        // Calculate paid amount
        var paidAmount = await _context.SessionPayments
            .Where(sp => sp.SessionId == sessionId)
            .SumAsync(sp => sp.Amount);

        // Calculate remaining amount
        var remainingAmount = sessionPrice - paidAmount;

        // Return DTO
        return new SessionBalanceDto {
            SessionId = sessionId,
            SessionPrice = sessionPrice,
            PaidAmount = paidAmount,
            RemainingAmount = remainingAmount,
            IsPaid = remainingAmount <= 0
        };
    }

    public async Task<AssignmentDto?> GetAssignmentAsync(int assignmentId) {
        // Check if assignment exists
        var assignment = await _context.SessionPayments.FindAsync(assignmentId);
        if (assignment == null) {
            return null;
        }
        
        // Return DTO
        return new AssignmentDto {
            Id = assignment.Id,
            SessionId = assignment.SessionId,
            PaymentId = assignment.PaymentId,
            Amount = assignment.Amount
        };
    }

    public async Task<AssignPaymentResponseDto> AssignPaymentToSessionsAsync(
        AssignPaymentRequestDto request) {
        // Check if payment exists
        var payment = await _context.Payments.FindAsync(request.PaymentId);
        if (payment == null) {
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Payment with ID {request.PaymentId} does not exist."
            };
        }

        // Check if there are any assignments
        if (!request.Assignments.Any()) {
            return new AssignPaymentResponseDto {
                Success = false,
                Message = "No session assignments provided."
            };
        }

        // Check if all sessions exist
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

        // Check if all sessions belong to the same client as the payment
        if (sessions.Any(s => s.ClientId != payment.ClientId)) {
            var wrongSessions = sessions.Where(s => s.ClientId != payment.ClientId)
                                    .Select(s => s.Id);
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Sessions with IDs {string.Join(", ", wrongSessions)} do not belong "
                    + $"to the same client as the payment with ID {payment.Id}."
            };
        }

        // Calculate the already assigned amount from this payment
        var alreadyAssigned = await _context.SessionPayments
            .Where(sp => sp.PaymentId == request.PaymentId)
            .SumAsync(sp => sp.Amount);

        // Calculate the new assigned amount
        var newAssignments = request.Assignments.Sum(a => a.Amount);

        // Check if we exceed the payment amount
        var totalAssigned = alreadyAssigned + newAssignments;
        if (totalAssigned > payment.Amount) {
            var remaining = payment.Amount - alreadyAssigned;
            return new AssignPaymentResponseDto {
                Success = false,
                Message = $"Cannot assign {newAssignments:0.##} CZK. Payment has "
                + $"{payment.Amount:0.##} CZK, already assigned {alreadyAssigned:0.##} CZK, "
                + $"only {remaining:0.##} CZK remaining."
            };
        }

        // Create SessionPayment records
        foreach (var assignment in request.Assignments) {
            var sessionPayment = new SessionPayment {
                SessionId = assignment.SessionId,
                PaymentId = request.PaymentId,
                Amount = assignment.Amount
            };
            _context.SessionPayments.Add(sessionPayment);
        }

        // Save to database
        await _context.SaveChangesAsync();

        // Return successful response
        return new AssignPaymentResponseDto {
            Success = true,
            Message = $"Payment successfully assigned to {request.Assignments.Count} session(s).",
            TotalAssignedAmount = newAssignments,
            RemainingPaymentAmount = payment.Amount - totalAssigned,
            AssignedSessionsCount = request.Assignments.Count
        };
    }

    public async Task<EditAssignmentResponseDto> EditAssignmentAsync(
        int assignmentId, EditAssignmentRequestDto request) {
        // Check if assignment exists
        var assignment = await _context.SessionPayments.FindAsync(assignmentId);
        if (assignment == null) {
            return new EditAssignmentResponseDto {
                Success = false,
                Message = $"Assignment with ID {assignmentId} does not exist."
            };
        }

        var oldAmount = assignment.Amount;

        // Get the payment
        var payment = await _context.Payments.FindAsync(assignment.PaymentId);
        if (payment == null) {
            return new EditAssignmentResponseDto {
                Success = false,
                Message = $"Payment with ID {assignment.PaymentId} does not exist."
            };
        }
        
        // Calculate already assigned amount (excluding current assignment)
        var otherAssignments = await _context.SessionPayments
            .Where(sp => sp.PaymentId == payment.Id && sp.Id != assignmentId)
            .SumAsync(sp => sp.Amount);

        // Check if new amount does not exceed payment amount
        var totalAssigned = otherAssignments + request.NewAmount;
        if (totalAssigned > payment.Amount) {
            var remaining = payment.Amount - otherAssignments;
            return new EditAssignmentResponseDto {
                Success = false,
                Message = $"Cannot assign {request.NewAmount:0.##} CZK. Payment has "
                + $"{payment.Amount:0.##} CZK, other assignments use {otherAssignments:0.##} CZK, "
                + $"only {remaining:0.##} CZK remaining."
            };
        }

        // Update assignment
        assignment.Amount = request.NewAmount;
        await _context.SaveChangesAsync();

        // Return success
        return new EditAssignmentResponseDto {
            Success = true,
            Message = $"Assignment successfully updated.",
            AssignmentId = assignmentId,
            OldAmount = oldAmount,
            NewAmount = request.NewAmount,
            RemainingPaymentAmount = payment.Amount - totalAssigned
        };
    }

    public async Task<bool> DeleteAssignmentAsync(int assignmentId) {
        // Check if assignment exists
        var assignment = await _context.SessionPayments.FindAsync(assignmentId);
        if (assignment == null) {
            return false;
        }

        // Delete assignment
        _context.SessionPayments.Remove(assignment);
        await _context.SaveChangesAsync();

        // Return success
        return true;
    }
}
