using ExpenseTrackerAPI.DTOs;
using ExpenseTrackerAPI.Extensions;
using ExpenseTrackerAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionsController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        // GET /api/transactions
        // GET /api/transactions?startDate=2026-01-01&endDate=2026-01-31
        // GET /api/transactions?type=EXPENSE
        // GET /api/transactions?categoryId=123
        // GET /api/transactions?page=1&pageSize=20
        [HttpGet]
        public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactions(
            [FromQuery] TransactionFilterParams filters)
        {
            var userId = User.GetUserId();
            var result = await _transactionService.GetTransactionsAsync(userId, filters);
            return Ok(result);
        }

        // GET /api/transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<TransactionDto>> GetTransaction(int id)
        {
            try
            {
                var userId = User.GetUserId();
                var transaction = await _transactionService.GetTransactionByIdAsync(userId, id);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST /api/transactions
        [HttpPost]
        public async Task<ActionResult<TransactionDto>> CreateTransaction(
            CreateTransactionRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var transaction = await _transactionService.CreateTransactionAsync(userId, request);

                return CreatedAtAction(
                    nameof(GetTransaction),
                    new { id = transaction.Id },
                    transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message); // 409 — type/category mismatch
            }
        }

        // PUT /api/transactions/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<TransactionDto>> UpdateTransaction(
            int id, UpdateTransactionRequest request)
        {
            try
            {
                var userId = User.GetUserId();
                var transaction = await _transactionService.UpdateTransactionAsync(userId, id, request);
                return Ok(transaction);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // DELETE /api/transactions/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            try
            {
                var userId = User.GetUserId();
                await _transactionService.DeleteTransactionAsync(userId, id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}