using System.Net.Http.Headers;
using System.Web;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Providers;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace LINE_DotNet_API.Domain
{
    public class LineLoginService
    {

        private static HttpClient client = new HttpClient();
        private readonly JsonProvider _jsonProvider = new JsonProvider();
        private readonly string loginUrl = "https://access.line.me/oauth2/v2.1/authorize?response_type={0}&client_id={1}&redirect_uri={2}&state={3}&scope={4}";
        private readonly string clientId = "2006807429";
        private readonly string clientSecret = "a10131800b5a9a3633bb8706fc9ba138";
        private readonly string tokenUrl = "https://api.line.me/oauth2/v2.1/token";
        private readonly string profileUrl = "https://api.line.me/v2/profile";
        private readonly string idTokenProfileUrl = "https://api.line.me/oauth2/v2.1/verify/?id_token={0}&client_id={1}";
        //private readonly ConnectionStrings _connectionStrings;
        private readonly AppDbContext _context;

        public LineLoginService(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // 回傳 line authorization url
        public string GetLoginUrl(string redirectUrl)
        {
            // 根據想要得到的資訊填寫 scope
            var scope = "profile%20openid%20email";
            // 這個 state 是隨便打的
            var state = "1qazRTGFDY5ysg";
            var uri = string.Format(loginUrl, "code", clientId, HttpUtility.UrlEncode(redirectUrl), state, scope);
            return uri;
        }

        // 取得 access token 等資料
        public async Task<TokensResponseDto> GetTokensByAuthToken(string authToken, string callbackUri)
        {
            var formContent = new FormUrlEncodedContent(new[]
            {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", authToken),
            new KeyValuePair<string, string>("redirect_uri",callbackUri),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
        });

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //添加 accept header
            var response = await client.PostAsync(tokenUrl, formContent); // 送出 post request
            var dto = _jsonProvider.Deserialize<TokensResponseDto>(await response.Content.ReadAsStringAsync()); //將 json response 轉成 dto

            return dto;
        }

        public async Task<userDataDto.UserProfileDto> GetUserProfileByAccessToken(string accessToken)
        {
            //取得 UserProfile
            var request = new HttpRequestMessage(HttpMethod.Get, profileUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await client.SendAsync(request);
            var profile = _jsonProvider.Deserialize<userDataDto.UserProfileDto>(await response.Content.ReadAsStringAsync());

            return profile;
        }

        public async Task<SubscribeDto> GetUserProfileByIdToken(string idToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, string.Format(idTokenProfileUrl, idToken, clientId));
            var response = await client.SendAsync(request);
            var dto = _jsonProvider.Deserialize<SubscribeDto>(await response.Content.ReadAsStringAsync());

            return dto;
        }

        public async Task<string> CheckAndSaveUser(USER userData)
        {
            if (userData == null)
            {
                throw new ArgumentNullException(nameof(userData), "❌ userData 為 null");
            }

            var existingUser = await _context.USERS
                .FirstOrDefaultAsync(u => u.USER_ID == userData.USER_ID);

            if (existingUser != null)
            {
                existingUser.LINE_ID = userData.LINE_ID;
                existingUser.LINE_DISPLAY_NAME = userData.LINE_DISPLAY_NAME;
                existingUser.COMBINE_LINE = 1;

                await _context.SaveChangesAsync();
            }

            return "Success";
        }
    }
}

