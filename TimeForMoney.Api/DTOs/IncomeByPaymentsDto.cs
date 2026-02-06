namespace TimeForMoney.Api.DTOs;

public class IncomeByPaymentsDto {
    public DateOnly PeriodFrom { get; set; }
    public DateOnly PeriodTo { get; set; }
    public int TotalPaymentsCount { get; set; }
    public decimal TotalIncome { get; set; }
}
