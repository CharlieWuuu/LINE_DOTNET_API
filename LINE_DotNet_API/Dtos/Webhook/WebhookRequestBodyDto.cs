namespace LINE_DotNet_API.Dtos
{
    /// <summary>
    /// ���� LINE �T�������㵲�c
    /// </summary>
    public class WebhookRequestBodyDto
    {
        public string? Destination { get; set; }
        public List<WebhookEventDto> Events { get; set; }
    }
}