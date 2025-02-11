using System.Net.Http.Headers;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Enum;
using LINE_DotNet_API.Providers;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace LINE_DotNet_API.Services
{
	public class RichMenuService
	{
		// 貼上 messaging api channel 中的 accessToken & secret
		private readonly string channelAccessToken = "1A6g/bX6X26ZyGzebLPuJle+1h0rmoyAA2gYVVMtoI6xO0D3chY23+t12STSrqtpdbQdDV8Jq9uWaIi+9QQazg+w7tKIeCcdZ24VotuOEC0Nr6zYORUf6RFooevsAOFd1C1eh2PHmyd+4zRoQHqh7AdB04t89/1O/w1cDnyilFU=";
		private readonly string channelSecret = "12f31f2841ecfbd6842fcca00040a451";
		private static HttpClient client = new HttpClient();
		private readonly JsonProvider _jsonProvider = new JsonProvider();
		private readonly string validateRichMenuUri = "https://api.line.me/v2/bot/richmenu/validate";
		private readonly string createRichMenuUri = "https://api.line.me/v2/bot/richmenu";
		private readonly string getRichMenuListUri = "https://api.line.me/v2/bot/richmenu/list";
		// {0} 的位置要帶入 richMenuId
		private readonly string richMenuImageUri = "https://api-data.line.me/v2/bot/richmenu/{0}/content";
		// {0} 的位置要帶入 richMenuId
		private readonly string setDefaultRichMenuUri = "https://api.line.me/v2/bot/user/all/richmenu/{0}";
		private readonly string createRichMenuAliasUri = "https://api.line.me/v2/bot/richmenu/alias";
		private readonly string commonRichMenuAliasUri = "https://api.line.me/v2/bot/richmenu/alias/{0}";
		private readonly string getRichMenuAliasListUri = "https://api.line.me/v2/bot/richmenu/alias/list";
		private readonly string getRichMenuUri = "https://api.line.me/v2/bot/richmenu/{0}";
		private readonly string defaultRichMenuUri = "https://api.line.me/v2/bot/user/all/richmenu";
		// 個人的 linked rich menu 操作
		private readonly string linkedRichMenuOfUserUri = "https://api.line.me/v2/bot/user/{0}/richmenu";
		// 多人的 linked rich menu 操作
		private readonly string linkedRichMenuOfMultipleUserUri = "https://api.line.me/v2/bot/richmenu/bulk/";

		public RichMenuService()
		{
		}

		public async Task<string> ValidateRichMenu(RichMenuDto richMenu)
		{
			var jsonBody = new StringContent(_jsonProvider.Serialize(richMenu), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(validateRichMenuUri),
				Content = jsonBody,
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> CreateRichMenu(RichMenuDto richMenu)
		{
			var jsonBody = new StringContent(_jsonProvider.Serialize(richMenu), Encoding.UTF8, "application/json");
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Post,
				RequestUri = new Uri(createRichMenuUri),
				Content = jsonBody,
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<RichMenuListDto> GetRichMenuList()
		{
			var request = new HttpRequestMessage
			{
				Method = HttpMethod.Get,
				RequestUri = new Uri(getRichMenuListUri),
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			var response = await client.SendAsync(request);

			Console.WriteLine(await response.Content.ReadAsStringAsync());
			var list = _jsonProvider.Deserialize<RichMenuListDto>(await response.Content.ReadAsStringAsync());
			// 依照名稱排序
			list.Richmenus = list.Richmenus.OrderBy((rm) => rm.Name).ToList();
			return list;
		}

		public async Task<string> UploadRichMenuImage(string richMenuId, IFormFile imageFile)
		{
			//判斷檔案格式 需為 png or jpeg
			if (!(Path.GetExtension(imageFile.FileName).Equals(".png", StringComparison.OrdinalIgnoreCase) || Path.GetExtension(imageFile.FileName).Equals(".jpeg", StringComparison.OrdinalIgnoreCase)))
			{
				return "圖片格式錯誤，須為 png or jpeg";
			}
			using (var stream = new MemoryStream())
			{
				//建立檔案內容
				imageFile.CopyTo(stream);
				var fileBytes = stream.ToArray();
				var content = new ByteArrayContent(fileBytes);
				content.Headers.ContentType = new MediaTypeHeaderValue("image/png");
				var request = new HttpRequestMessage(HttpMethod.Post, String.Format(richMenuImageUri, richMenuId))
				{
					Content = content
				};
				client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
				var response = await client.SendAsync(request);

				return await response.Content.ReadAsStringAsync();
			}
		}

		public async Task<string> SetDefaultRichMenu(string richMenuId)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, String.Format(setDefaultRichMenuUri, richMenuId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> CreateRichMenuAlias(RichMenuAliasDto richMenuAlias)
		{

			var jsonBody = new StringContent(_jsonProvider.Serialize(richMenuAlias), Encoding.UTF8, "application/json");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			var request = new HttpRequestMessage(HttpMethod.Post, createRichMenuAliasUri)
			{
				Content = jsonBody
			};

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> DeleteRichMenuAlias(string richMenuAliasId)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(commonRichMenuAliasUri, richMenuAliasId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> UpdateRichMenuAlias(string richMenuAliasId, string richMenuId)
		{
			var body = new { richMenuId = richMenuId };
			var jsonBody = new StringContent(_jsonProvider.Serialize(body), Encoding.UTF8, "application/json");
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			var request = new HttpRequestMessage(HttpMethod.Post, string.Format(commonRichMenuAliasUri, richMenuAliasId))
			{
				Content = jsonBody
			};

			var response = await client.SendAsync(request);
			return await response.Content.ReadAsStringAsync();
		}

		public async Task<RichMenuAliasDto> GetRichMenuAliasInfo(string richMenuAliasId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, string.Format(commonRichMenuAliasUri, richMenuAliasId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
				return _jsonProvider.Deserialize<RichMenuAliasDto>(await response.Content.ReadAsStringAsync());
			else
				return new RichMenuAliasDto();
		}

		public async Task<RichMenuAliasListDto> GetRichMenuAliasListInfo()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, getRichMenuAliasListUri);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			if (response.StatusCode == System.Net.HttpStatusCode.OK)
				return _jsonProvider.Deserialize<RichMenuAliasListDto>(await response.Content.ReadAsStringAsync());
			else
				return new RichMenuAliasListDto();
		}

		public async Task<FileContentResult> DownloadRichMenuImage(string richMenuId)
		{
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);
			// 直接取得回傳的 Byte Array
			var bytes = await client.GetByteArrayAsync(String.Format(richMenuImageUri, richMenuId));
			//
			return new FileContentResult(bytes, "image/png");
		}

		public async Task<string> DeleteRichMenu(string richMenuId)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(getRichMenuUri, richMenuId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		// 回傳物件直接使用 RichMenuDto，就不另外宣告了。
		public async Task<RichMenuDto> GetRichMenuById(string richMenuId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, string.Format(getRichMenuUri, richMenuId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);
			var richMenu = _jsonProvider.Deserialize<RichMenuDto>(await response.Content.ReadAsStringAsync());
			return richMenu;
		}

		public async Task<RichMenuDto> GetDefaultRichMenuId()
		{
			var request = new HttpRequestMessage(HttpMethod.Get, defaultRichMenuUri);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return _jsonProvider.Deserialize<RichMenuDto>(await response.Content.ReadAsStringAsync());
		}

		public async Task<string> CancelDefaultRichMenu()
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, defaultRichMenuUri);
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<RichMenuDto> GetRichMenuIdLinkedToUser(string userId)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, string.Format(linkedRichMenuOfUserUri, userId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return _jsonProvider.Deserialize<RichMenuDto>(await response.Content.ReadAsStringAsync());
		}

		public async Task<string> LinkRichMenuToUser(string userId, string richMenuId)
		{

			var request = new HttpRequestMessage(HttpMethod.Post, string.Format(linkedRichMenuOfUserUri + "/{1}", userId, richMenuId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> UnlinkRichMenuFromUser(string userId)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, string.Format(linkedRichMenuOfUserUri, userId));
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> LinkRichMenuToMultipleUser(LinkRichMenuToMultipleUserDto dto)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, linkedRichMenuOfMultipleUserUri + "link")
			{
				Content = new StringContent(_jsonProvider.Serialize(dto), Encoding.UTF8, "application/json")
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}

		public async Task<string> UnlinkRichMenuFromMultipleUser(LinkRichMenuToMultipleUserDto dto)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, linkedRichMenuOfMultipleUserUri + "unlink")
			{
				Content = new StringContent(_jsonProvider.Serialize(dto), Encoding.UTF8, "application/json")
			};
			client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken);

			var response = await client.SendAsync(request);

			return await response.Content.ReadAsStringAsync();
		}
	}
}
