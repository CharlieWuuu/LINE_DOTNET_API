using LINE_DotNet_API.Enum;

namespace LINE_DotNet_API.Dtos
{
	public class ImageMessageDto : BaseMessageDto
	{
		public ImageMessageDto()
		{
			Type = MessageTypeEnum.Image;
		}

		public string OriginalContentUrl { get; set; }
		public string PreviewImageUrl { get; set; }
	}
}
