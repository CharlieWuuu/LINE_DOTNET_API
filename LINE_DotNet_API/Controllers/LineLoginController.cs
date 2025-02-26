using LINE_DotNet_API.Domain;
using LINE_DotNet_API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace LINE_DotNet_API.Controllers
{
	[Route("api/[Controller]")]
	[ApiController]
	public class LineLoginController : ControllerBase
	{
		private readonly LineLoginService _lineLoginService;

		public LineLoginController(LineLoginService lineLoginService)
		{
			_lineLoginService = lineLoginService ?? throw new ArgumentNullException(nameof(lineLoginService));
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

		// // 使用 id token 取得 user profile
		// [HttpPost("CheckUserCombineLine")]
		// public async Task<bool> CheckAndSaveUser(USER userData)
		// {
		//     return await _lineLoginService.CheckUserCombineLine(userData);
		// }

		// 使用 id token 取得 user profile
		[HttpPost("SendVerifyCode")]
		public async Task<bool> SendVerifyCode(USER userData)
		{
			return await _lineLoginService.SendVerifyCode(userData);
		}

		// 使用 id token 取得 user profile
		[HttpPost("CheckVerifyCode")]
		public async Task<bool> CheckVerifyCode(EMAIL_VERIFICATION emailVerification)
		{
			return await _lineLoginService.CheckVerifyCode(emailVerification);
		}

		[HttpPost("LoginUser")]
		public async Task<string> LoginUser(USER userData)
		{
			return await _lineLoginService.LoginUser(userData);
		}
	}
}
