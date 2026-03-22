using ExpenseTrackerAPI.DTOs.Reports;
using ExpenseTrackerAPI.Extensions;
using ExpenseTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET /api/reports/summary/monthly
        // GET /api/reports/summary/monthly?year=2026
        // GET /api/reports/summary/monthly?year=2026&month=3
        [HttpGet("summary/monthly")]
        public async Task<ActionResult<List<MonthlySummaryDto>>> GetMonthlySummary(
            [FromQuery] ReportFilterParams filters)
        {
            var userId = User.GetUserId();
            var result = await _reportService.GetMonthlySummaryAsync(userId, filters);

            if (!result.Any())
                return Ok(new List<MonthlySummaryDto>()); // Return empty list, not 404

            return Ok(result);
        }

        // GET /api/reports/summary/yearly
        // GET /api/reports/summary/yearly?year=2026
        [HttpGet("summary/yearly")]
        public async Task<ActionResult<List<YearlySummaryDto>>> GetYearlySummary(
            [FromQuery] ReportFilterParams filters)
        {
            var userId = User.GetUserId();
            var result = await _reportService.GetYearlySummaryAsync(userId, filters);

            if (!result.Any())
                return Ok(new List<YearlySummaryDto>());

            return Ok(result);
        }

        // GET /api/reports/category-breakdown
        // GET /api/reports/category-breakdown?type=EXPENSE
        // GET /api/reports/category-breakdown?type=INCOME&startDate=2026-01-01&endDate=2026-03-31
        [HttpGet("category-breakdown")]
        public async Task<ActionResult<List<CategoryBreakdownDto>>> GetCategoryBreakdown(
            [FromQuery] ReportFilterParams filters)
        {
            // Validate type param if provided
            if (!string.IsNullOrWhiteSpace(filters.Type) &&
                filters.Type.ToUpper() != "INCOME" &&
                filters.Type.ToUpper() != "EXPENSE")
            {
                return BadRequest("Type must be either 'INCOME' or 'EXPENSE'.");
            }

            var userId = User.GetUserId();
            var result = await _reportService.GetCategoryBreakdownAsync(userId, filters);

            if (!result.Any())
                return Ok(new List<CategoryBreakdownDto>());

            return Ok(result);
        }

        // GET /api/reports/cashflow
        // GET /api/reports/cashflow?startDate=2026-01-01&endDate=2026-01-31
        [HttpGet("cashflow")]
        public async Task<ActionResult<CashflowDto>> GetCashflow(
            [FromQuery] ReportFilterParams filters)
        {
            // Validate date range logic
            if (filters.StartDate.HasValue && filters.EndDate.HasValue &&
                filters.StartDate > filters.EndDate)
            {
                return BadRequest("StartDate cannot be later than EndDate.");
            }

            var userId = User.GetUserId();
            var result = await _reportService.GetCashflowAsync(userId, filters);
            return Ok(result);
        }
    }
}