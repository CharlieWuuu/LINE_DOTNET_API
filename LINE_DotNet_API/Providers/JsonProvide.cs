using System.Text.Json;
using System.Text.Json.Serialization;

namespace LINE_DotNet_API.Providers
{
	public class JsonProvider
	{
		private JsonSerializerOptions serializeOption = new JsonSerializerOptions()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
			PropertyNameCaseInsensitive = true,
			DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
			Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
		};
	
		private static JsonSerializerOptions deserializeOptions = new JsonSerializerOptions
		{
			PropertyNameCaseInsensitive = true,
		};

		public string Serialize<T>(T obj)
		{
			return JsonSerializer.Serialize(obj, serializeOption);
		}

		public T Deserialize<T>(string str)
		{
			return JsonSerializer.Deserialize<T>(str, deserializeOptions);
		}
	}
}
