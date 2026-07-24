using HotelManagementSystem.Models.Payment;

namespace HotelManagementSystem.Interfaces.PaymentInterface
{
    public interface IPaymentService
    {
        //Task<int> CreatePaymentAsync(Payment pay);
        //Task<bool> UpdatePaymentAsync(Payment pay);
        Task<Payment?> GetPaymentByUuidAsync(string transactionUuid);
    }
    public interface IPaymentDLL
    {
        Task<int> CreatePaymentAsync(Payment pay);
        Task<bool> UpdatePaymentAsync(Payment pay);
        Task<Payment?> GetPaymentByUuidAsync(string transactionUuid);
    } 
}
