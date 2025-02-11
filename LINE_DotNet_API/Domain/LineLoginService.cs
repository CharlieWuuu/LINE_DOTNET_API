using System.Net.Http.Headers;
using System.Text;
using System.Web;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Providers;
using Microsoft.Data.SqlClient;

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
        // private readonly ConnectionStrings _connectionStrings;

        public LineLoginService(ConnectionStrings connectionStrings)
        {
            // _connectionStrings = connectionStrings;
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

        //public async Task<string> CheckAndSaveUser(userDataDto userData)
        //{
        //    string checkUserQuery = @"SELECT COUNT(*) FROM [userProfile] WHERE [userId] = @userId";
        //    string insertUserQuery = @"
        //INSERT INTO [userProfile] ([userId], [displayName], [pictureUrl], [statusMessage], [CREATE_TIME])
        //VALUES (@userId, @displayName, @pictureUrl, @statusMessage, GETDATE())";
        //    string insertLoginLogQuery = @"
        //INSERT INTO [idTokenProfile] ([userId], [amr], [aud], [email], [exp], [iat], [iss], [name], [picture], [sub], [isInClient], [language], [lineVersion], [os], [version], [LOGIN_TIME])
        //VALUES (@userId, @amr, @aud, @email, @exp, @iat, @iss, @name, @picture, @sub, @isInClient, @language, @lineVersion, @os, @version, GETDATE())";

        //    using (var connection = new SqlConnection(_connectionStrings.DefaultConnection))
        //    {
        //        try
        //        {
        //            await connection.OpenAsync();

        //            // 檢查 userProfile 表
        //            using (var checkCommand = new SqlCommand(checkUserQuery, connection))
        //            {
        //                checkCommand.Parameters.Add(new SqlParameter("@userId", userData.userProfile.UserId));
        //                int userCount = (int)await checkCommand.ExecuteScalarAsync();

        //                // 如果 userProfile 不存在，插入新資料
        //                if (userCount == 0)
        //                {
        //                    using (var insertCommand = new SqlCommand(insertUserQuery, connection))
        //                    {
        //                        insertCommand.Parameters.Add(new SqlParameter("@userId", userData.userProfile.UserId));
        //                        insertCommand.Parameters.Add(new SqlParameter("@displayName", userData.userProfile.DisplayName ?? (object)DBNull.Value));
        //                        insertCommand.Parameters.Add(new SqlParameter("@pictureUrl", userData.userProfile.PictureUrl ?? (object)DBNull.Value));
        //                        insertCommand.Parameters.Add(new SqlParameter("@statusMessage", userData.userProfile.StatusMessage ?? (object)DBNull.Value));
        //                        await insertCommand.ExecuteNonQueryAsync();
        //                    }
        //                }
        //            }

        //            // 插入登入紀錄到 idTokenProfile 表
        //            using (var insertLoginCommand = new SqlCommand(insertLoginLogQuery, connection))
        //            {
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@userId", userData.userProfile.UserId));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@amr", userData.idTokenProfile.amr?.FirstOrDefault() ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@aud", userData.idTokenProfile.aud ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@email", userData.idTokenProfile.email ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@exp", userData.idTokenProfile.exp));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@iat", userData.idTokenProfile.iat));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@iss", userData.idTokenProfile.iss ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@name", userData.idTokenProfile.name ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@picture", userData.idTokenProfile.picture ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@sub", userData.idTokenProfile.sub ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@isInClient", userData.environment.isInClient));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@language", userData.environment.language ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@lineVersion", userData.environment.lineVersion ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@os", userData.environment.os ?? (object)DBNull.Value));
        //                insertLoginCommand.Parameters.Add(new SqlParameter("@version", userData.environment.version ?? (object)DBNull.Value));
        //                await insertLoginCommand.ExecuteNonQueryAsync();
        //            }
        //            return "Success";
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception("Database error", ex);
        //        }
        //    }
        //}
    }
}
