using LINE_DotNet_API.Enum;
namespace LINE_DotNet_API.Dtos
{
	public class VideoMessageDto : BaseMessageDto
	{
		public VideoMessageDto()
		{
			Type = MessageTypeEnum.Video;
		}

		public string OriginalContentUrl { get; set; }
		public string PreviewImageUrl { get; set; }
		public string? TrackingId { get; set; }
	}
}
