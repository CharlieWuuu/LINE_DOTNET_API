namespace LINE_DotNet_API.Dtos
{
	public class SubscribeDto
	{
		public string? Iss { get; set; }
		public string? Sub { get; set; }
		public string? Aud { get; set; }
		public int? Exp { get; set; }
		public int? Auth_time { get; set; }
		public int? Iat { get; set; }
		public string? Nonce { get; set; }
		public string[]? Amr { get; set; }
		public string? Name { get; set; }
		public string? Picture { get; set; }
		public string? Email { get; set; }
	}
}
