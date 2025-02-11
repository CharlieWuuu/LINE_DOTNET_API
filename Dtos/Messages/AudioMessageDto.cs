using LINE_DotNet_API.Enum;
namespace LINE_DotNet_API.Dtos
{
	public class AudioMessageDto : BaseMessageDto
	{
		public AudioMessageDto()
		{
			Type = MessageTypeEnum.Audio;
		}

		public string OriginalContentUrl { get; set; }
		public int Duration { get; set; }
	}
}
