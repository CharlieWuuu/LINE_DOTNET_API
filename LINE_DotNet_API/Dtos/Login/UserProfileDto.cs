/// <summary>
/// �N��ϥΪ̸�T
/// </summary>
/// 

public class userDataDto
{
    public UserProfileDto userProfile { get; set; } // UserProfileDto �@���ݩ�

    public idTokenProfileDto idTokenProfile { get; set; } // idTokenProfileDto �@���ݩ�

    public environmentDto environment { get; set; } // environmentDto �@���ݩ�

    /// <summary>
    /// �ϥΪ̰򥻸�T
    /// </summary>
    public class UserProfileDto
    {
        /// <summary>
        /// �ϥΪ̪��ߤ@�ѧO�X
        /// </summary>
        public string UserId { get; set; }
        public string DisplayName { get; set; }
        public string StatusMessage { get; set; }
        public string PictureUrl { get; set; }
    }

    /// <summary>
    /// �ϥΪ̨�L��T
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
    /// �ϥΪ̵n�J����
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
