using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Models.Bill;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.BillController
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillSeervice _billService;

        public BillController(IBillSeervice billService)
        {
            _billService = billService;
        }


        [HttpGet("view-bill/{sessionId}")]
        public async Task<IActionResult> GetBillPreview(int sessionId, [FromQuery] decimal discountPercentage)
        {
            if (_billService == null)
                return StatusCode(500, "Server configuration error: bill service not initialized.");

            if (sessionId <= 0)
                return BadRequest("Invalid Session ID.");

            if (discountPercentage < 0 || discountPercentage > 100)
                return BadRequest("Discount percentage must be between 0 and 100.");

            try
            {
                var billPreview = await _billService.CalculateSessionTotalAsync(sessionId, discountPercentage);

                if (billPreview == null)
                    return NotFound($"No active orders found for Session ID {sessionId} to calculate a bill.");

                return Ok(billPreview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("pay")]
        public async Task<IActionResult> PayBill([FromBody] PayBill pay)
        {
            try
            {
                // This will execute your math calculations and return a preview object DTO
                var billPreview = await _billService.PayBill(pay);

                if (billPreview == null)
                    return NotFound($"No active orders found for Session ID {pay} to calculate a bill.");

                return Ok(new{message = "successfully bill paid",ReviewBill =  billPreview});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        //public async Task<IActionResult> PayBill([FromBody] BillPaymentRequest request)
        //{
        //    if (request == null)
        //        return BadRequest("Payment request body cannot be null.");

        //    try
        //    {
        //        // Calls the updated service to finalize numbers and trigger BillDLL.CreateBillAsync
        //        Bill? finalizedBill = await _billService.GenerateAndPayBillAsync(
        //            request.SessionId,
        //            request.SubTotalAmount,
        //            request.DiscountPercentage,
        //            request.PaymentMethod,
        //            request.UserId
        //        );

        //        if (finalizedBill == null)
        //            return StatusCode(500, "Failed to commit the transaction record.");

        //        return Ok(new { message = "Bill paid and closed successfully.", receipt = finalizedBill });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        //[HttpGet("verify/{billNo}")]
        //public async Task<IActionResult> VerifyBill(int billNo)
        //{
        //    if (billNo <= 0)
        //        return BadRequest("Invalid Bill Number.");

        //    try
        //    {
        //        // Ensure you expose a matching lookup call down through your BillService/DLL layers
        //        Bill? archivedBill = await _billService.GetBillByNoAsync(billNo);

        //        if (archivedBill == null)
        //            return NotFound(new { verified = false, message = $"Invoice #{billNo} was not found inside the active records." });

        //        return Ok(new
        //        {
        //            verified = true,
        //            message = "Invoice record verified successfully.",
        //            billDetails = archivedBill
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"Internal server error: {ex.Message}");
        //    }
        //}
        //public class BillPaymentRequest
        //{
        //    public int SessionId { get; set; }
        //    public decimal SubTotalAmount { get; set; }
        //    public decimal DiscountPercentage { get; set; }
        //    public string PaymentMethod { get; set; } = "Cash";
        //    public int UserId { get; set; }
        //}

        
    }
}

