using System;
using System.Net.Http.Headers;
using System.Text;
using LINE_DotNet_API.Dtos;
using LINE_DotNet_API.Enum;
using LINE_DotNet_API.Providers;

namespace LINE_DotNet_API.Services
{
    public class LineBotService
    {

        // (將 LineBotController 裡宣告的 ChannelAccessToken & ChannelSecret 移到 LineBotService中)
        // 貼上 messaging api channel 中的 accessToken & secret
        private readonly string channelAccessToken = "1A6g/bX6X26ZyGzebLPuJle+1h0rmoyAA2gYVVMtoI6xO0D3chY23+t12STSrqtpdbQdDV8Jq9uWaIi+9QQazg+w7tKIeCcdZ24VotuOEC0Nr6zYORUf6RFooevsAOFd1C1eh2PHmyd+4zRoQHqh7AdB04t89/1O/w1cDnyilFU=";
        private readonly string channelSecret = "12f31f2841ecfbd6842fcca00040a451";

        // 宣告變數
        private readonly string replyMessageUri = "https://api.line.me/v2/bot/message/reply";
        private readonly string broadcastMessageUri = "https://api.line.me/v2/bot/message/broadcast";
        private readonly string multicastMessageUri = "https://api.line.me/v2/bot/message/multicast";
        
        private static HttpClient client = new HttpClient(); // 負責處理HttpRequest
        private readonly JsonProvider _jsonProvider = new JsonProvider();

        public LineBotService()
        {
        }

        public void ReceiveWebhook(WebhookRequestBodyDto requestBody)
        {
            foreach (var eventObject in requestBody.Events)
            {
                switch (eventObject.Type)
                {
                    case WebhookEventTypeEnum.Message:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室說了「{eventObject.Message.Text}」！");
                        ReceiveMessageWebhookEvent(eventObject); // 自動回覆訊息               
                        break;
                    case WebhookEventTypeEnum.Unsend:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}在聊天室收回訊息！");
                        break;
                    case WebhookEventTypeEnum.Follow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}將我們新增為好友！");
                        break;
                    case WebhookEventTypeEnum.Unfollow:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}封鎖了我們！");
                        break;
                    case WebhookEventTypeEnum.Join:
                        Console.WriteLine("我們被邀請進入聊天室了！");
                        break;
                    case WebhookEventTypeEnum.Leave:
                        Console.WriteLine("我們被聊天室踢出了");
                        break;
                    case WebhookEventTypeEnum.MemberJoined:
                        string joinedMemberIds = "";
                        foreach (var member in eventObject.Joined.Members)
                        {
                            joinedMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{joinedMemberIds}加入了群組！");
                        break;
                    case WebhookEventTypeEnum.MemberLeft:
                        string leftMemberIds = "";
                        foreach (var member in eventObject.Left.Members)
                        {
                            leftMemberIds += $"{member.UserId} ";
                        }
                        Console.WriteLine($"使用者{leftMemberIds}離開了群組！");
                        break;
                    case WebhookEventTypeEnum.Postback:
                        Console.WriteLine($"使用者{eventObject.Source.UserId}觸發了postback事件");
                        break;
                    case WebhookEventTypeEnum.VideoPlayComplete:
                        var replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventObject.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto(){Text = $"使用者您好，謝謝您收看我們的宣傳影片，祝您身體健康萬事如意 !"}
                            }
                        };
                        ReplyMessageHandler("text", replyMessage);
                        break;
                }
            }
        }
        /// <summary>
        /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void BroadcastMessageHandler(string messageType, object requestBody)
        {
            string strBody = requestBody.ToString();
            dynamic messageRequest = new BroadcastMessageRequestDto<BaseMessageDto>();
            switch (messageType)
            {
                case MessageTypeEnum.Text:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<TextMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Sticker:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<StickerMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Image:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImageMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Video:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<VideoMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Audio:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<AudioMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Location:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<LocationMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Imagemap:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<ImagemapMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.FlexBubble:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>>(strBody);
                    break;
                case MessageTypeEnum.FlexCarousel:
                    messageRequest = _jsonProvider.Deserialize<BroadcastMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>>>(strBody);
                    break;
            }
            BroadcastMessage(messageRequest);
        }

        /// <summary>
        /// 將廣播訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void BroadcastMessage<T>(BroadcastMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(broadcastMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 接收到廣播請求時，在將請求傳至 Line 前多一層處理，依據收到的 messageType 將 messages 轉換成正確的型別，這樣 Json 轉換時才能正確轉換。
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void MulticastMessageHandler(string messageType, object requestBody)
        {
            string strBody = requestBody.ToString();
            dynamic messageRequest = new MulticastMessageRequestDto<BaseMessageDto>();
            switch (messageType)
            {
                case MessageTypeEnum.Text:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<TextMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Sticker:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<StickerMessageDto>>(strBody);
                    break;

                case MessageTypeEnum.Image:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<ImageMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Video:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<VideoMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Audio:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<AudioMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Location:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<LocationMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.Imagemap:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<ImagemapMessageDto>>(strBody);
                    break;
                case MessageTypeEnum.FlexBubble:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<FlexMessageDto<FlexBubbleContainerDto>>>(strBody);
                    break;
                case MessageTypeEnum.FlexCarousel:
                    messageRequest = _jsonProvider.Deserialize<MulticastMessageRequestDto<FlexMessageDto<FlexCarouselContainerDto>>>(strBody);
                    break;
            }
            MulticastMessage(messageRequest);
        }

        /// <summary>
        /// 將分群訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void MulticastMessage<T>(MulticastMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(multicastMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        /// <summary>
        /// 接收到回覆請求時，在將請求傳至 Line 前多一層處理(目前為預留)
        /// </summary>
        /// <param name="messageType"></param>
        /// <param name="requestBody"></param>
        public void ReplyMessageHandler<T>(string messageType, ReplyMessageRequestDto<T> requestBody)
        {
            ReplyMessage(requestBody);
        }

        /// <summary>
        /// 將回覆訊息請求送到 Line
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="request"></param>
        public async void ReplyMessage<T>(ReplyMessageRequestDto<T> request)
        {
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", channelAccessToken); //帶入 channel access token
            var json = _jsonProvider.Serialize(request);
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(replyMessageUri),
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(requestMessage);
            Console.WriteLine(await response.Content.ReadAsStringAsync());
        }

        private void ReceiveMessageWebhookEvent(WebhookEventDto eventDto)
        {
            dynamic replyMessage = new ReplyMessageRequestDto<BaseMessageDto>();

            switch (eventDto.Message.Type)
            {
                // 收到文字訊息
                case MessageTypeEnum.Text:
                    // 訊息內容等於 "測試" 時
                    if (eventDto.Message.Text == "測試")
                    {
                        //這裡給各位自由發揮，建立一個 ReplyMessageRequestDto 吧。
                        //不過下方也提供了一個我回覆的訊息範例，需要的請參考～
                        // 回覆文字訊息並挾帶 quick reply button
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "QuickReply 測試訊息",
                                    QuickReply = new QuickReplyItemDto
                                    {
                                        Items = new List<QuickReplyButtonDto>
                                        {
                                            // message action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Message,
                                                    Label = "message 測試" ,
                                                    Text = "測試"
                                                }
                                            },
                                            // uri action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "uri 測試" ,
                                                    Uri = "https://www.richitech.com.tw/"
                                                }
                                            },
                                             // 使用 uri schema 分享 Line Bot 資訊
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "分享 Line Bot 資訊" ,
                                                    Uri = "https://line.me/R/nv/recommendOA/@070qbplr"
                                                }
                                            },
                                            // postback action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Postback,
                                                    Label = "postback 測試" ,
                                                    Data = "quick reply postback action" ,
                                                    DisplayText = "使用者傳送 displayTex，但不會有 Webhook event 產生。",
                                                    InputOption = PostbackInputOptionEnum.OpenKeyboard,
                                                    FillInText = "第一行\n第二行"
                                                }
                                            },
                                            // datetime picker action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                Type = ActionTypeEnum.DatetimePicker,
                                                Label = "日期時間選擇",
                                                    Data = "quick reply datetime picker action",
                                                    Mode = DatetimePickerModeEnum.Datetime,
                                                    Initial = "2025-02-01T19:00",
                                                    Max = "2030-12-31T23:59",
                                                    Min = "2024-01-01T00:00"
                                                }
                                            },
                                            // camera action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Camera,
                                                    Label = "開啟相機"
                                                }
                                            },
                                            // camera roll action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.CameraRoll,
                                                    Label = "開啟相簿"
                                                }
                                            },
                                            // location action
                                            new QuickReplyButtonDto {
                                                Action = new ActionDto {
                                                    Type = ActionTypeEnum.Location,
                                                    Label = "開啟位置"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Sender"
                    else if (eventDto.Message.Text == "Sender")
                    {
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                            {
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 1號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 1號",
                                        IconUrl = "https://cdn-icons-png.flaticon.com/512/11498/11498793.png"
                                    }
                                },
                                new TextMessageDto
                                {
                                    Text = "你好，我是客服人員 2號",
                                    Sender = new SenderDto
                                    {
                                        Name = "客服人員 2號",
                                        IconUrl = "https://cdn-icons-png.flaticon.com/512/6997/6997674.png"
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Buttons"
                    else if (eventDto.Message.Text == "Buttons")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ButtonsTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ButtonsTemplateDto>>
                            {
                                new TemplateMessageDto<ButtonsTemplateDto>
                                {
                                    AltText = "這是按鈕模板訊息",
                                    Template = new ButtonsTemplateDto
                                    {
                                        // 此處使用的是 Imgur 上圖片的絕對路徑
                                        ThumbnailImageUrl = "https://i.imgur.com/CP6ywwc.jpg",
                                        ImageAspectRatio = TemplateImageAspectRatioEnum.Rectangle,
                                        ImageSize = TemplateImageSizeEnum.Cover,
                                        Title = "親愛的用戶您好，歡迎您使用本美食推薦系統",
                                        Text = "請選擇今天想吃的主食，系統會推薦附近的餐廳給您。",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=sushi",
                                                Label = "壽司",
                                                DisplayText = "壽司"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=hot-pot",
                                                Label = "火鍋",
                                                DisplayText = "火鍋"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=steak",
                                                Label = "牛排",
                                                DisplayText = "牛排"
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "foodType=next",
                                                Label = "下一個",
                                                DisplayText = "下一個"
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Confirm"
                    else if (eventDto.Message.Text == "Confirm")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ConfirmTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ConfirmTemplateDto>>
                            {
                                new TemplateMessageDto<ConfirmTemplateDto>
                                {
                                    AltText = "這是確認模組訊息",
                                    Template = new ConfirmTemplateDto
                                    {
                                        Text = "請問您是否喜歡本產品?\n(產品編號123)",
                                        Actions = new List<ActionDto>
                                        {
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "id=123&like=yes",
                                                Label = "喜歡",
                                                DisplayText = "喜歡",
                                            },
                                            new ActionDto
                                            {
                                                Type = ActionTypeEnum.Postback,
                                                Data = "id=123&like=no",
                                                Label = "不喜歡",
                                                DisplayText = "不喜歡",
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "Carousel"
                    else if (eventDto.Message.Text == "Carousel")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<CarouselTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<CarouselTemplateDto>>
                            {
                                new TemplateMessageDto<CarouselTemplateDto>
                                {
                                    AltText = "這是輪播訊息",
                                    Template = new CarouselTemplateDto
                                    {
                                        Columns = new List<CarouselColumnObjectDto>
                                        {
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Title = "全新上市 iPhone 16 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    //按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                    }
                                                }
                                            },
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Title = "全新上市 iPhone 16 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    //按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                    }
                                                }
                                            },
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Title = "全新上市 iPhone 16 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    //按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                    }
                                                }
                                            },
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Title = "全新上市 iPhone 16 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    //按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                    }
                                                }
                                            },
                                            new CarouselColumnObjectDto
                                            {
                                                ThumbnailImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Title = "全新上市 iPhone 16 Pro",
                                                Text = "現在購買享優惠，全品項 9 折",
                                                Actions = new List<ActionDto>
                                                {
                                                    //按鈕 action
                                                    new ActionDto
                                                    {
                                                        Type = ActionTypeEnum.Uri,
                                                        Label ="立即購買",
                                                        Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 關鍵字 : "ImageCarousel"
                    else if (eventDto.Message.Text == "ImageCarousel")
                    {
                        replyMessage = new ReplyMessageRequestDto<TemplateMessageDto<ImageCarouselTemplateDto>>
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TemplateMessageDto<ImageCarouselTemplateDto>>
                            {
                                new TemplateMessageDto<ImageCarouselTemplateDto>
                                {
                                    AltText = "這是圖片輪播訊息",
                                    Template = new ImageCarouselTemplateDto
                                    {
                                        Columns = new List<ImageCarouselColumnObjectDto>
                                        {
                                            new ImageCarouselColumnObjectDto
                                            {
                                                ImageUrl = "https://www.apple.com/v/iphone-16-pro/d/images/meta/iphone-16-pro_overview__ejy873nl8yi6_og.png?202501080137",
                                                Action = new ActionDto
                                                {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "前往官網",
                                                    Uri = "https://www.apple.com/tw/iphone-16-pro/"
                                                }
                                            },
                                            new ImageCarouselColumnObjectDto
                                            {
                                                ImageUrl = "https://i4.momoshop.com.tw/1734719768/goodsimg/0013/259/420/13259420_OR.webp",
                                                Action = new ActionDto
                                                {
                                                    Type = ActionTypeEnum.Uri,
                                                    Label = "立即購買",
                                                    Uri = "https://www.apple.com/tw/shop/buy-iphone/iphone-16-pro"
                                                }
                                            },

                                        }
                                    }
                                }
                            }
                        };
                    }
                    // 其餘任何文字訊息
                    else
                    {
                        replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                        {
                            ReplyToken = eventDto.ReplyToken,
                            Messages = new List<TextMessageDto>
                                {
                                    new TextMessageDto(){Text = $"您好，您傳送了 {eventDto.Message.Type}：「{eventDto.Message.Text}」！"}
                                }
                        };
                    }
                    break;
                default:
                    var replyText = $"{eventDto.Message.Type}";
                    replyText += $"：「{eventDto.Message.Text}」";
                    replyMessage = new ReplyMessageRequestDto<TextMessageDto>()
                    {
                        ReplyToken = eventDto.ReplyToken,
                        Messages = new List<TextMessageDto>
                                {
                                    new TextMessageDto(){Text = $"您好，您傳送了 {eventDto.Message.Type}！"}
                                }
                    };
                    break;
            }
            ReplyMessageHandler(eventDto.Message.Type, replyMessage);
        }       
    }
}
