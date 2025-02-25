using System.Threading.Tasks;
using LINE_DotNet_API.Dtos;
using Microsoft.EntityFrameworkCore;


namespace LINE_DotNet_API.Services
{
    public class SubscriptionService
    {
        private readonly AppDbContext _context;

        public SubscriptionService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // 取得訂閱資訊
        public async Task<SubscriptionDto> Fetch(SUBSCRIBE dto)
        {
            var subscription = await _context.SUBSCRIBES
                .Where(s => s.USER_ID == dto.USER_ID)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return new SubscriptionDto
                {
                    USER_ID = dto.USER_ID,
                    OCEAN_POLLUTION = 0,
                    OIL_REPORT = 0,
                    SATELLITE_IMAGES = 0
                };
            }

            return new SubscriptionDto
            {
                USER_ID = subscription.USER_ID,
                OCEAN_POLLUTION = subscription.OCEAN_POLLUTION,
                OIL_REPORT = subscription.OIL_REPORT,
                SATELLITE_IMAGES = subscription.SATELLITE
            };
        }

        public async Task Update(SUBSCRIBE subscribe)
        {
            // 先檢查 USERS 表中是否有這個 USER_ID
            var existingUser = await _context.USERS
                .Where(u => u.USER_ID == subscribe.USER_ID)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception($"❌ 找不到 USER_ID = {subscribe.USER_ID}，無法更新訂閱記錄！");
            }

            // 查詢是否已經有訂閱記錄
            var existingSubscription = await _context.SUBSCRIBES
                .Where(s => s.USER_ID == subscribe.USER_ID)
                .FirstOrDefaultAsync();

            if (existingSubscription == null)
            {
                // 插入新的訂閱記錄
                var newSubscription = new SUBSCRIBE
                {
                    USER_ID = subscribe.USER_ID,
                    OCEAN_POLLUTION = subscribe.OCEAN_POLLUTION,
                    OIL_REPORT = subscribe.OIL_REPORT,
                    SATELLITE = subscribe.SATELLITE,
                };

                _context.SUBSCRIBES.Add(newSubscription);
            }
            else
            {
                // 更新現有訂閱記錄
                existingSubscription.OCEAN_POLLUTION = subscribe.OCEAN_POLLUTION;
                existingSubscription.OIL_REPORT = subscribe.OIL_REPORT;
                existingSubscription.SATELLITE = subscribe.SATELLITE;

                _context.SUBSCRIBES.Update(existingSubscription);
            }

            await _context.SaveChangesAsync();
        }
    }
}
