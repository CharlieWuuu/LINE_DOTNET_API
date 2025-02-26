using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
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

        // /// <summary>
        // /// 檢查使用者是否已經綁定 LINE
        // /// </summary>
        // public async Task<bool> CheckUserCombineLine(USER userData)
        // {
        //     if (userData == null)
        //     {
        //         throw new ArgumentNullException(nameof(userData), "❌ userData 為 null");
        //     }

        //     var existingUser = await _context.USERS
        //         .FirstOrDefaultAsync(u => u.LINE_ID == userData.LINE_ID);

        //     if (existingUser == null)
        //     {
        //         return false;
        //     }
        //     else
        //     {
        //         return true;
        //     }
        // }

        /// <summary>
        /// 發送驗證碼到使用者信箱
        /// </summary>
        public async Task<bool> SendVerifyCode(USER userData)
        {
            if (userData == null || string.IsNullOrWhiteSpace(userData.EMAIL))
            {
                throw new ArgumentException("❌ userData 為 null 或 EMAIL 為空");
            }

            var existingEmail = await _context.USERS.FirstOrDefaultAsync(u => u.EMAIL == userData.EMAIL);

            if (existingEmail != null)
            {

                // 產生 6 位數驗證碼
                var verifyCode = new Random().Next(100000, 999999).ToString();

                // 儲存驗證碼到資料庫
                var verifyEntry = new EMAIL_VERIFICATION
                {
                    EMAIL = userData.EMAIL,
                    CODE = verifyCode,
                    EXPIRES_AT = DateTime.UtcNow.AddMinutes(5), // 設定 5 分鐘內有效
                    IS_VERIFIED = 0
                };

                _context.EMAIL_VERIFICATIONS.Add(verifyEntry);
                await _context.SaveChangesAsync();

                bool emailSent = await SendEmailAsync(verifyEntry);

                return emailSent;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 檢查使用者輸入的驗證碼
        /// </summary>
        public async Task<bool> CheckVerifyCode(string EMAIL, string CODE, string LINE_ID, string LINE_DISPLAY_NAME)
        {
            var verifyEntry = await _context.EMAIL_VERIFICATIONS
                .FirstOrDefaultAsync((v => v.EMAIL == EMAIL && v.CODE == CODE));

            if (verifyEntry == null || verifyEntry.EXPIRES_AT < DateTime.UtcNow)
            {
                return false; // 驗證碼不存在或已過期
            }

            // 設定驗證成功
            verifyEntry.IS_VERIFIED = 1;
            await _context.SaveChangesAsync();

            // 儲存 COMBINE_LINE
            var existingUser = await _context.USERS.FirstOrDefaultAsync(u => u.EMAIL == EMAIL);
            if (existingUser != null)
            {
                existingUser.LINE_ID = LINE_ID;
                existingUser.LINE_DISPLAY_NAME = LINE_DISPLAY_NAME;
                existingUser.COMBINE_LINE = 1;
            }
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 儲存或更新使用者資料，並紀錄登入紀錄
        /// </summary>
        public async Task<bool> LoginUser(USER userData)
        {
            if (userData == null)
            {
                throw new ArgumentNullException(nameof(userData), "❌ userData 為 null");
            }

            var existingUser = await _context.USERS
                .FirstOrDefaultAsync(u => u.LINE_ID == userData.LINE_ID);

            if (existingUser != null)
            {
                // 更新 LINE 資訊
                existingUser.LINE_DISPLAY_NAME = userData.LINE_DISPLAY_NAME;

                // 紀錄登入時間（存 UTC 時間，讀取時轉回當地時間）
                var newLogin = new USER_LOGIN
                {
                    USER_ID = existingUser.USER_ID,
                    LOGIN_TIME = DateTime.UtcNow // 以 UTC 儲存
                };

                _context.USER_LOGINS.Add(newLogin);
                await _context.SaveChangesAsync();
            }
            else
            {
                return false;
            }

            return true;
        }

        private async Task<bool> SendEmailAsync(EMAIL_VERIFICATION emailVerification)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587, // 或 465 (SSL)
                    Credentials = new NetworkCredential("charliewu500@gmail.com", "wquxvwremdazsvtr"),
                    EnableSsl = true,
                };


                var mailMessage = new MailMessage
                {
                    From = new MailAddress("charliewu500@gmail.com"),
                    Subject = "您的驗證碼",
                    Body = $"您的驗證碼為：{emailVerification.CODE}，請在 5 分鐘內輸入完成驗證。",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(emailVerification.EMAIL);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        //public async Task<Boolean> CheckUserCombineLine(USER userData)
        //{
        //    if (userData == null)
        //    {
        //        throw new ArgumentNullException(nameof(userData), "❌ userData 為 null");
        //    }

        //    var existingUser = await _context.USERS
        //        .FirstOrDefaultAsync(u => u.EMAIL == userData.EMAIL); await _context.SaveChangesAsync();

        //    if (existingUser.COMBINE_LINE == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public async Task<Boolean> SendVerifyCode(USER userData)
        //{

        //}

        //public async Task<Boolean> CheckVerifyCode(USER userData)
        //{

        //}

        //public async Task<string> SaveUser(USER userData)
        //{
        //    if (userData == null)
        //    {
        //        throw new ArgumentNullException(nameof(userData), "❌ userData 為 null");
        //    }

        //    var existingUser = await _context.USERS
        //        .FirstOrDefaultAsync(u => u.EMAIL == userData.EMAIL);

        //    if (existingUser != null)
        //    {
        //        existingUser.LINE_ID = userData.LINE_ID;
        //        existingUser.LINE_DISPLAY_NAME = userData.LINE_DISPLAY_NAME;
        //        existingUser.COMBINE_LINE = 1;

        //        var newLogin = new USER_LOGIN
        //        {
        //            USER_ID = existingUser.USER_ID,
        //            LOGIN_TIME = DateTime.UtcNow,
        //        };

        //        _context.USER_LOGINS.Add(newLogin);
        //        await _context.SaveChangesAsync();
        //    }

        //    return "Success";
        //}
    }
}
