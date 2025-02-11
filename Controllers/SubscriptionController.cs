using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Services;
using Microsoft.AspNetCore.Mvc;

namespace LINE_DotNet_API.Controllers
{	
	[Route("api/[Controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
	{
		private readonly SubscriptionService _subscribeService;
        private readonly ConnectionStrings _connectionStrings;
        public SubscriptionController(ConnectionStrings connectionStrings)
		{
            _subscribeService = new SubscriptionService(connectionStrings);
		}

        // 取得用戶訂閱資訊 
        [HttpPost("Fetch")]
        public async Task<SubscriptionDto> Fetch(SubscriptionDto dto)
        {
			return await _subscribeService.Fetch(dto);
        }

        [HttpPost("Update")]
		public async Task Update(SubscriptionDto dto)
		{
			await _subscribeService.Update(dto);
		}
	}
}
