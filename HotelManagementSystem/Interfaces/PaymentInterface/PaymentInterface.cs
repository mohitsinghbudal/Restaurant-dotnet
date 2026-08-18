using HotelManagementSystem.Models.Payment;

namespace HotelManagementSystem.Interfaces.PaymentInterface
{
    public interface IPaymentService
    {
        
        
        Task<Payment?> GetPaymentByUuidAsync(string transactionUuid);
        Task<IEnumerable<Payment>> GetALLPaymentsAsync();
    }
    public interface IPaymentDLL
    {
        Task<int> CreatePaymentAsync(Payment pay);
        Task<bool> UpdatePaymentAsync(Payment pay);
        Task<Payment?> GetPaymentByUuidAsync(string transactionUuid);
        Task<IEnumerable<Payment>> GetALLPaymentsAsync();
    } 
}

