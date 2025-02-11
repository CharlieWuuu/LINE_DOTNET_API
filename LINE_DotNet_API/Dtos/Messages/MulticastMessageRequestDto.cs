namespace LINE_DotNet_API.Dtos
{
	public class MulticastMessageRequestDto<T>
	{
		public string[] to { get; set; }
        public List<T> Messages { get; set; }
		public bool? NotificationDisabled { get; set; }
	}
}
