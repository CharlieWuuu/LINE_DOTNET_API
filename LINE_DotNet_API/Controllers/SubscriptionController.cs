using LINE_DotNet_API.Domain;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace LINE_DotNet_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly SubscriptionService _subscribeService;

        public SubscriptionController(SubscriptionService subscribeService)
        {
            _subscribeService = subscribeService ?? throw new ArgumentNullException(nameof(subscribeService));
        }

        // 取得訂閱資訊
        [HttpPost("Fetch")]
        public async Task<SUBSCRIBE> Fetch([FromBody] SUBSCRIBE dto)
        {
            return await _subscribeService.Fetch(dto);
        }

        // 更新訂閱資訊
        [HttpPost("Update")]
        public async Task<bool> Update([FromBody] SUBSCRIBE subscribe)
        {
            return await _subscribeService.Update(subscribe);
        }
    }
}
