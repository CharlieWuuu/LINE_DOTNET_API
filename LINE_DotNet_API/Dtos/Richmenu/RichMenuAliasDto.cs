namespace LINE_DotNet_API.Dtos
{
	public class RichMenuAliasListDto
	{
		public RichMenuAliasDto[] Aliases { get; set; }
	}
	public class RichMenuAliasDto
	{
		public string RichMenuAliasId { get; set; }
		public string RichMenuId { get; set; }
	}
}
