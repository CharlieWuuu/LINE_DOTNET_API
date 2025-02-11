namespace LINE_DotNet_API.Dtos
{
    /// <summary>
    /// 對應 LINE 訊息的完整結構
    /// </summary>
    public class WebhookRequestBodyDto
    {
        public string? Destination { get; set; }
        public List<WebhookEventDto> Events { get; set; }
    }
}