using System.ComponentModel.DataAnnotations;

namespace LINE_DotNet_API.Dtos
{
    public class ConnectionStrings
    {

        [Required]
        public string? DefaultConnection { get; set; }
    }
}
