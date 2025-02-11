/// <summary>
/// 代表使用者資訊
/// </summary>
/// 

public class userDataDto
{
    public UserProfileDto userProfile { get; set; } // UserProfileDto 作為屬性

    public idTokenProfileDto idTokenProfile { get; set; } // idTokenProfileDto 作為屬性

    public environmentDto environment { get; set; } // environmentDto 作為屬性

    /// <summary>
    /// 使用者基本資訊
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// 使用者的唯一識別碼
        /// </summary>
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string StatusMessage { get; set; }
        public string PictureUrl { get; set; }
    }

    /// <summary>
    /// 使用者其他資訊
    /// </summary>
    public class idTokenProfileDto
    {
        public string[] amr { get; set; }
        public string aud { get; set; }
        public string email { get; set; }
        public int exp { get; set; }
        public int iat { get; set; }
        public string iss { get; set; }
        public string name { get; set; }
        public string picture { get; set; }
        public string sub { get; set; }
    }

    /// <summary>
    /// 使用者登入環境
    /// </summary>
    public class environmentDto
    {
        public int isInClient { get; set; }
        public string language { get; set; }
        public string lineVersion { get; set; }
        public string os { get; set; }
        public string version { get; set; }
    }
}
