using HotelManagementSystem.Interfaces.Redis;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedisController : ControllerBase
    {
        private readonly IRedisService _redis;

        public RedisController(IRedisService redis)
        {
            _redis = redis;
        }

        [HttpGet("test")]
        public async Task<IActionResult> Test()
        {
            await _redis.SetAsync(
                "test:key",
                "Hello Redis!",
                TimeSpan.FromMinutes(5));

            var value =
                await _redis.GetAsync<string>("test:key");

            return Ok(new
            {
                message = value
            });
        }
    }
}