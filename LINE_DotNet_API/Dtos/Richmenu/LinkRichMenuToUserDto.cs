namespace LINE_DotNet_API.Dtos
{
	public class LinkRichMenuToMultipleUserDto
	{
		public string? RichMenuId { get; set; }
		public List<string> UserIds { get; set; }
	}
}
