using HotelManagementSystem.DLL.PaymentDLL;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Models.Payment;

namespace HotelManagementSystem.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentDLL _paydll;
        public PaymentService ( IPaymentDLL paydll)
        {
            _paydll= paydll;
        }
       //public async Task<int> CreatePaymentAsync(Payment pay) 
       // {
            
       // }
       // public async Task<bool> UpdatePaymentAsync(Payment pay) 
       // {
            

       // }
        public async Task<Payment?> GetPaymentByUuidAsync(string transactionUuid) 
        {
            return await _paydll.GetPaymentByUuidAsync(transactionUuid);
        }
    }
}
