using DocumentFormat.OpenXml.Bibliography;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Models.Bill;
using HotelManagementSystem.Models.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers.BillController
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpGet("view-bill/{sessionId}")]
        public async Task<IActionResult> ViewBillAsync(int sessionId)
        {
            try
            {
                var existingbill = await _billService.ViewBillAsync(sessionId);

                return Ok(existingbill);
            }catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("calculate-bill/{sessionId}")]
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

        [HttpPost("pay/cash")]
        public async Task<IActionResult> PayBill([FromBody] PayBill pay)
        {
            try
            {
                // This will execute your math calculations and return a preview object DTO
                var billPreview = await _billService.PayBillCash(pay);

                if (billPreview == null)
                    return NotFound($"No active orders found for Session ID {pay} to calculate a bill.");

                return Ok(new{message = "successfully bill paid",ReviewBill =  billPreview});
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("pay/esewa/${SessionId}")]
        public async Task<IActionResult> PayBillEsewa([FromBody] EsewaInitiate req)
        {
            try
            {
                var paidbill = await _billService.InitiateEsewaPaymentAsync(req.SessionId);

                
                return Ok(paidbill);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("esewa-callback")]
        public async Task<IActionResult> EsewaCallback([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { message = "Missing response data." });
            }

            bool isVerified = await _billService.VerifyAndProcessEsewaCallbackAsync(data);

            if (isVerified)
            {
                return Ok(new { status = "Success", message = "Payment verified and bill marked as paid." });
            }

            return BadRequest(new { status = "Failed", message = "Payment verification failed or status incomplete." });
        }

        //[HttpGet("pay/esewa/success")]
        //public async Task<IActionResult> Success(string data)
        //{

        //    return Ok(new { message = "successfull" });
            
        //}
        [HttpGet("pay/esewa/success")]
        public async Task<IActionResult> EsewaSuccess([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { message = "Missing response payload." });
            }

            bool isVerified = await _billService.VerifyAndProcessEsewaCallbackAsync(data);

            if (isVerified)
            {
                return Ok(new { status = "Success", message = "Payment verified successfully." });
            }

            return BadRequest(new { status = "Failed", message = "Payment verification failed." });
        }

        [HttpGet("pay/esewa/failure")]
        public IActionResult EsewaFailure()
        {
            return BadRequest(new { status = "Failed", message = "Payment was canceled or failed at eSewa portal." });
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
    public class EsewaInitiate
    {
        public int SessionId { get; set; }
    }
}

