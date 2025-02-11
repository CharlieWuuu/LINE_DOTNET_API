using LINE_DotNet_API.Domain;
using LINE_DotNet_API.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace LINE_DotNet_API.Controllers
{
	[Route("api/[Controller]")]
	[ApiController]
	public class LineLoginController : ControllerBase
	{
		private readonly LineLoginService _lineLoginService;
        private readonly ConnectionStrings _connectionStrings;
        public LineLoginController(ConnectionStrings connectionStrings)
		{
			_lineLoginService = new LineLoginService(connectionStrings);
        }

		// 取得 Line Login 網址
		[HttpGet("Url")]
		public string GetLoginUrl([FromQuery] string redirectUrl)
		{
			return _lineLoginService.GetLoginUrl(redirectUrl);
		}

		// 使用 authToken 取回登入資訊
		[HttpGet("Tokens")]
		public async Task<TokensResponseDto> GetTokensByAuthToken([FromQuery] string authToken, [FromQuery] string callbackUrl)
		{
			return await _lineLoginService.GetTokensByAuthToken(authToken, callbackUrl);
		}

		// 使用 access token 取得 user profile
		[HttpGet("Profile/{accessToken}")]
		public async Task<userDataDto.UserProfileDto> GetUserProfileByAccessToken(string accessToken)
		{
			return await _lineLoginService.GetUserProfileByAccessToken(accessToken);
		}

		// 使用 id token 取得 user profile
		[HttpGet("Profile/IdToken/{idToken}")]
		public async Task<SubscribeDto> GetUserProfileByIdToken(string idToken)
		{
			return await _lineLoginService.GetUserProfileByIdToken(idToken);
		}

        // 使用 id token 取得 user profile
        [HttpPost("CheckAndSaveUser")]
        public async Task<string> CheckAndSaveUser(userDataDto userData)
        {
            return await _lineLoginService.CheckAndSaveUser(userData);
        }
    }
}
