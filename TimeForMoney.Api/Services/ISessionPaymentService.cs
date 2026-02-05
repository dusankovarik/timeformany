namespace TimeForMoney.Api.Services;

using TimeForMoney.Api.DTOs;

public interface ISessionPaymentService {
    Task<SessionPaymentStatusDto?> GetSessionPaymentStatusAsync(int sessionId);
}
