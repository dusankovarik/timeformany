namespace TimeForMoney.Api.DTOs;

public class IncomeBySessionsDto {
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public int TotalSessionsCount { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal PaidIncome { get; set; }
    public decimal UnpaidIncome { get; set; }
}
