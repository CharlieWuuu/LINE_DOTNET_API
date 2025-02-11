using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace LINE_DotNet_API.Controllers
{
	[Route("api/[Controller]")]
	[ApiController]
    [Tags("上傳圖片")]
    public class FileController : ControllerBase
	{

		private readonly IConfiguration _configuration;
		public FileController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Only for Imagemap message 讀取圖片
		/// </summary>
		/// <param name="folderName">檔案名稱</param>
		/// <param name="imageSize">圖片尺寸</param>
		/// <returns></returns>
		[HttpGet("UploadFiles/ImagemapImages/{folderName}/{imageSize}")]
		public IActionResult GetImagemapImage(string folderName, string imageSize)
		{
			//FileStream fs = new FileStream();
			var path = $"{_configuration.GetValue<string>(WebHostDefaults.ContentRootKey)}/UploadFiles/ImagemapImages/{folderName}/{imageSize}.png";
			using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
			{
				var fileBytes = System.IO.File.ReadAllBytes(path);
				new FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(path), out var contentType);
				return new FileContentResult(fileBytes, contentType ?? "application/octet-stream");
			}
		}
	}
}
