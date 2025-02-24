using System.ComponentModel.DataAnnotations;

namespace LINE_DotNet_API.Dtos
{
	public class SUBSCRIBE
	{
		[Key]
		[MaxLength(100)]
		public string USER_ID { get; set; }
		public int OCEAN_POLLUTION { get; set; }
		public int OIL_REPORT { get; set; }
		public int SATELLITE { get; set; }
	}
}
