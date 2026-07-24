using DocumentFormat.OpenXml.EMMA;
using HotelManagementSystem.Interfaces.PaymentInterface;
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
                var result = await _payserve.GetPaymentByUuidAsync(uuid);
                //if (!result)
                //{
                //    throw new Exception("bad request");
                //}
                return Ok(new {payment = result });
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
