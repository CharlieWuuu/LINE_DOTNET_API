namespace LINE_DotNet_API.Dtos
{
    public class EMAIL_VERIFICATION
    {
        public string? EMAIL { get; set; }
        public string? CODE { get; set; }
        public DateTime? EXPIRES_AT { get; set; }
        public int? IS_VERIFIED { get; set; }
        public string? LINE_ID { get; set; }
        public string? LINE_DISPLAY_NAME { get; set; }
    }
}
