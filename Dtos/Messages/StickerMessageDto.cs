using LINE_DotNet_API.Enum;

namespace LINE_DotNet_API.Dtos
{
	public class StickerMessageDto : BaseMessageDto
	{
		public StickerMessageDto()
		{
			Type = MessageTypeEnum.Sticker;
		}
		public string PackageId { get; set; }
		public string StickerId { get; set; }
	}
}
