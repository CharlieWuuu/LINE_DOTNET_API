namespace LINE_DotNet_API.Dtos
{
    public class SubscriptionDto
    {
        public string USER_ID { get; set; } // 使用者 ID
        public int? OCEAN_POLLUTION { get; set; } // 海洋汙染事件
        public int? OIL_REPORT { get; set; } // 影像油汙報告
        public int? SATELLITE_IMAGES { get; set; } // 最新衛星影像
    }
}