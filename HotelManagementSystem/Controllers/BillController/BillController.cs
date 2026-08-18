using DocumentFormat.OpenXml.Bibliography;
using HotelManagementSystem.Helper.ClaimHelper;
using HotelManagementSystem.Interfaces.BillInterface;
using HotelManagementSystem.Models.Bill;
using HotelManagementSystem.Models.Payment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelManagementSystem.Controllers.BillController
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BillController : ControllerBase
    {
        private readonly IBillService _billService;

        public BillController(IBillService billService)
        {
            _billService = billService;
        }

        [HttpGet("view-bill")]
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
                int userId = ClaimHelper.GetUserId(User);
                pay.PaidBy = userId;
                
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

        [HttpPost("pay/esewa")]
        public async Task<IActionResult> PayBillEsewa([FromQuery] int req)
        {
            try
            {
                var paidbill = await _billService.InitiateEsewaPaymentAsync(req);

                
                return Ok(paidbill);
            }
            catch(Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [AllowAnonymous]
        [HttpGet("esewa-callback")]
        public async Task<IActionResult> EsewaCallback([FromQuery] string data)
        {

            int userId = ClaimHelper.GetUserId(User);
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { message = "Missing response data." });
            }

            bool isVerified = await _billService.VerifyAndProcessEsewaCallbackAsync(data,userId);

            if (isVerified)
            {
                return Ok(new { status = "Success", message = "Payment verified and bill marked as paid." });
            }

            return BadRequest(new { status = "Failed", message = "Payment verification failed or status incomplete." });
        }

        
        
        

        

        
        [AllowAnonymous]
        [HttpGet("pay/esewa/success")]
        public async Task<IActionResult> EsewaSuccess([FromQuery] string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                return BadRequest(new { message = "Missing response payload." });
            }
            int userId = ClaimHelper.GetUserId(User);

            bool isVerified = await _billService.VerifyAndProcessEsewaCallbackAsync(data, userId);

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

        [HttpGet("all-bills")]
        public async Task<IActionResult> GetBillAsync()
        {
            try
            {
                if (!User.IsInRole("5")) 
                {
                    throw new Exception("User not allowed");
                }

                var bills = await _billService.GetBillAsync();

                return Ok(new { allbills = bills });
            }catch(Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        
        
        
        

        
        
        
        
        
        
        
        
        
        

        
        

        
        
        
        
        
        
        
        
        
        
        
        

        
        
        
        

        
        

        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        
        


    }
    public class EsewaInitiate
    {
        public int SessionId { get; set; }
    }
}


