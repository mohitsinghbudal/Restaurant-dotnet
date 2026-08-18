using DocumentFormat.OpenXml.EMMA;
using HotelManagementSystem.Interfaces.PaymentInterface;
using HotelManagementSystem.Models.Payment;
using HotelManagementSystem.Services.PaymentService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.PaymentController
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _payserve;
        public PaymentController(IPaymentService payserve)
        {
            _payserve = payserve;
        }
        [HttpGet]
        public async Task<IActionResult> GetPaymentInfo([FromQuery]string uuid)
        {
            try
            {
                if (!User.IsInRole("5")) 
                {
                    throw new Exception("User not allowed");
                }
                var result = await _payserve.GetPaymentByUuidAsync(uuid);
                
                
                
                
                return Ok(new {payment = result });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("all")]
        public async Task<IActionResult> GetALLPaymentsAsync()
        {
            try
            {
                if (!User.IsInRole("5")) 
                {
                    throw new Exception("User not allowed");
                }
                var result = await _payserve.GetALLPaymentsAsync();
                return Ok(new { message = result });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex });
            }
        }
    }
}

