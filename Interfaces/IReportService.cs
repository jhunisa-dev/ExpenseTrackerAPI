using ExpenseTrackerAPI.DTOs.Reports;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Interfaces
{
    public interface IReportService
    {
        Task<List<MonthlySummaryDto>> GetMonthlySummaryAsync(int userId, ReportFilterParams filters);
        Task<List<YearlySummaryDto>> GetYearlySummaryAsync(int userId, ReportFilterParams filters);
        Task<List<CategoryBreakdownDto>> GetCategoryBreakdownAsync(int userId, ReportFilterParams filters);
        Task<CashflowDto> GetCashflowAsync(int userId, ReportFilterParams filters);
    }
}