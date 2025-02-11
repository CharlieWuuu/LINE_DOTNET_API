using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Services;
using LINE_DotNet_API.Providers;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;


namespace LINE_DotNet_API.Controllers
{

    [Route("api/[Controller]")]
    [ApiController]
    public class LineBotController : ControllerBase
    {
        // 宣告 service
        private readonly LineBotService _lineBotService;
        private readonly RichMenuService _richMenuService;
        private readonly JsonProvider _jsonProvider;
        // constructor
        public LineBotController()
        {
            _lineBotService = new LineBotService();
            _richMenuService = new RichMenuService();
            _jsonProvider = new JsonProvider();
        }

        /// <summary>偵測使用者行為，並顯示在終端機；若為訊息則會自動回覆</summary>
        /// <param name="body"></param>
        /// <returns></returns>
        [HttpPost("Webhook")]
        public IActionResult Webhook(WebhookRequestBodyDto body)
        {
            _lineBotService.ReceiveWebhook(body); // 呼叫 Service
            return Ok();
        }

        /// <summary>對所有用戶廣播訊息</summary>
        /// <param name="messageType">訊息類型，範例：text </param>
        /// <param name="body">要傳送的訊息</param>
        [HttpPost("SendMessage/Broadcast")]
        public IActionResult Broadcast([Required] string messageType, object body)
        {
            _lineBotService.BroadcastMessageHandler(messageType, body);
            return Ok();
        }

        /// <summary>分眾傳訊息</summary>
        [HttpPost("SendMessage/Multicast")]
        public IActionResult Multicast([Required] string messageType, object body)
        {
            _lineBotService.MulticastMessageHandler(messageType, body);
            return Ok();
        }        

        //rich menu api
        [HttpPost("RichMenu/Validate")]
        public async Task<IActionResult> ValidateRichMenu(RichMenuDto richMenu)
        {
            return Ok(await _richMenuService.ValidateRichMenu(richMenu));
        }

        [HttpPost("RichMenu/Create")]
        public async Task<IActionResult> CreateRichMenu(RichMenuDto richMenu)
        {
            return Ok(await _richMenuService.CreateRichMenu(richMenu));
        }

        [HttpGet("RichMenu/GetList")]
        public async Task<IActionResult> GetRichMenuList()
        {
            return Ok(await _richMenuService.GetRichMenuList());
        }

        [HttpPost("RichMenu/UploadImage/{richMenuId}")]
        public async Task<IActionResult> UploadRichMenuImage(IFormFile imageFile, string richMenuId)
        {
            return Ok(await _richMenuService.UploadRichMenuImage(richMenuId, imageFile));
        }

        [HttpGet("RichMenu/SetDefault/{richMenuId}")]
        public async Task<IActionResult> SetDefaultRichMenu(string richMenuId)
        {
            return Ok(await _richMenuService.SetDefaultRichMenu(richMenuId));
        }
        //Rich menu alias
        [HttpPost("RichMenu/Alias/Create")]
        public async Task<IActionResult> CreateRichMenuAlias(RichMenuAliasDto richMenuAlias)
        {
            return Ok(await _richMenuService.CreateRichMenuAlias(richMenuAlias));
        }

        [HttpDelete("RichMenu/Alias/Delete/{richMenuAliasId}")]
        public async Task<IActionResult> DeleteRichMenuAlias(string richMenuAliasId)
        {
            return Ok(await _richMenuService.DeleteRichMenuAlias(richMenuAliasId));
        }

        [HttpPost("RichMenu/Alias/Upadte/{richMenuAliasId}")]
        public async Task<IActionResult> UpdateRichMenuAlias(string richMenuAliasId, string richMenuId)
        {
            return Ok(await _richMenuService.UpdateRichMenuAlias(richMenuAliasId, richMenuId));
        }

        [HttpGet("RichMenu/Alias/GetInfo/{richMenuAliasId}")]
        public async Task<IActionResult> GetRichMenuAliasInfomation(string richMenuAliasId)
        {
            return Ok(await _richMenuService.GetRichMenuAliasInfo(richMenuAliasId));
        }

        [HttpGet("RichMenu/Alias/GetInfo/List")]
        public async Task<IActionResult> GetRichMenuAliasList()
        {
            return Ok(await _richMenuService.GetRichMenuAliasListInfo());
        }

        [HttpGet("RichMenu/DownloadImage/{richMenuId}")]
        public async Task<FileContentResult> DownloadRichMenuImageById(string richMenuId)
        {
            return await _richMenuService.DownloadRichMenuImage(richMenuId);
        }

        [HttpDelete("RichMenu/Delete/{richMenuId}")]
        public async Task<IActionResult> DeleteRichMenu(string richMenuId)
        {
            return Ok(await _richMenuService.DeleteRichMenu(richMenuId));
        }

        [HttpGet("RichMenu/Get/{richMenuId}")]
        public async Task<IActionResult> GetRichMenuById(string richMenuId)
        {
            return Ok(await _richMenuService.GetRichMenuById(richMenuId));
        }

        [HttpGet("RichMenu/Default/GetId")]
        public async Task<IActionResult> GetDefaultRichMenuId()
        {
            return Ok(await _richMenuService.GetDefaultRichMenuId());
        }

        [HttpGet("RichMenu/Default/Cancel")]
        public async Task<IActionResult> CancelDefaultRichMenu()
        {
            return Ok(await _richMenuService.CancelDefaultRichMenu());
        }

        [HttpGet("RichMenu/GetLinkedId/{userId}")]
        public async Task<IActionResult> GetRichMenuIdLinkedToUser(string userId)
        {
            return Ok(await _richMenuService.GetRichMenuIdLinkedToUser(userId));
        }

        [HttpGet("RichMenu/Link/{userId}/{richMenuId}")]
        public async Task<IActionResult> LinkRichMenuToUser(string userId, string richMenuId)
        {
            return Ok(await _richMenuService.LinkRichMenuToUser(userId, richMenuId));
        }

        [HttpDelete("RichMenu/Unlink/{userId}")]
        public async Task<IActionResult> UnlinkRichMenuFromUser(string userId)
        {
            return Ok(await _richMenuService.UnlinkRichMenuFromUser(userId));
        }

        [HttpPost("RichMenu/Link/Multiple")]
        public async Task<IActionResult> LinkRichMenuToMultipleUser(LinkRichMenuToMultipleUserDto dto)
        {
            return Ok(await _richMenuService.LinkRichMenuToMultipleUser(dto));
        }

        [HttpPost("RichMenu/Unlink/Multiple")]
        public async Task<IActionResult> UnlinkRichMenuFromMMultipleUser(LinkRichMenuToMultipleUserDto dto)
        {
            return Ok(await _richMenuService.UnlinkRichMenuFromMultipleUser(dto));
        }
    }
}
