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
        public async Task<SUBSCRIBE> Fetch(SUBSCRIBE dto)
        {
            var subscription = await _context.SUBSCRIBES
                .Where(s => s.LINE_ID == dto.LINE_ID)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                return new SUBSCRIBE
                {
                    LINE_ID = dto.LINE_ID,
                    OCEAN_POLLUTION = 0,
                    OIL_REPORT = 0,
                    SATELLITE = 0
                };
            }

            return new SUBSCRIBE
            {
                LINE_ID = subscription.LINE_ID,
                OCEAN_POLLUTION = subscription.OCEAN_POLLUTION,
                OIL_REPORT = subscription.OIL_REPORT,
                SATELLITE = subscription.SATELLITE
            };
        }

        public async Task<bool> Update(SUBSCRIBE subscribe)
        {
            // 先檢查 USERS 表中是否有這個 LINE_ID
            var existingUser = await _context.USERS
                .Where(u => u.LINE_ID == subscribe.LINE_ID)
                .FirstOrDefaultAsync();

            if (existingUser == null)
            {
                throw new Exception($"❌ 找不到 LINE_ID = {subscribe.LINE_ID}，無法更新訂閱記錄！");
            }

            // 查詢是否已經有訂閱記錄
            var existingSubscription = await _context.SUBSCRIBES
                .Where(s => s.LINE_ID == subscribe.LINE_ID)
                .FirstOrDefaultAsync();

            if (existingSubscription == null)
            {
                // 插入新的訂閱記錄
                var newSubscription = new SUBSCRIBE
                {
                    LINE_ID = subscribe.LINE_ID,
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

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"❌ 更新訂閱記錄時發生錯誤：{ex.Message}");
            }
        }
    }
}
