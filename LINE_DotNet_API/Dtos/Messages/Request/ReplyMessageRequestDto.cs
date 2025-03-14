namespace LINE_DotNet_API.Dtos
{
	public class ReplyMessageRequestDto<T>
	{
		public string ReplyToken { get; set; }
		public List<T> Messages { get; set; }
		public bool? NotificationDisabled { get; set; }
	}
}
