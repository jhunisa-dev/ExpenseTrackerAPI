using ExpenseTrackerAPI.DTOs;
using System.Threading.Tasks;

namespace ExpenseTrackerAPI.Interfaces
{
    public interface ITransactionService
    {
        Task<PagedResult<TransactionDto>> GetTransactionsAsync(int userId, TransactionFilterParams filters);
        Task<TransactionDto> GetTransactionByIdAsync(int userId, int transactionId);
        Task<TransactionDto> CreateTransactionAsync(int userId, CreateTransactionRequest request);
        Task<TransactionDto> UpdateTransactionAsync(int userId, int transactionId, UpdateTransactionRequest request);
        Task DeleteTransactionAsync(int userId, int transactionId);
    }
}