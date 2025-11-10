using NapPlana.Core.Bot;
using NapPlana.Core.Data;
using NapPlana.Core.Data.API;
using NapPlana.Core.Data.Message;
using NapPlana.Core.Event.Handler;
using System.IO; // for FileStream, File, Path

var bot = BotFactory
    .Create()
    .SetConnectionType(BotConnectionType.WebSocketClient)
    .SetIp("172.17.21.238")
    .SetPort(6100)
    .SetToken("plana-bot")
    .Build();
BotEventHandler.OnLogReceived += (level, message) =>
{
    if (level == LogLevel.Debug)
    {
        return;
    }
    Console.WriteLine($"[{level}] {message}");
};
BotEventHandler.OnMessageSentGroup += (eventData) =>
{
    Console.WriteLine($"消息类型 {eventData.MessageType}, 消息ID: {eventData.MessageId}");
};


var cts = new CancellationTokenSource();
Console.CancelKeyPress += async (s, e) =>
{
    e.Cancel = true;
    await bot.StopAsync();
    cts.Cancel();
};

await bot.StartAsync();

// 发送信息测试（包含 FileStream -> base64 图片示例）
var builder = MessageChainBuilder.Create()
    .AddMentionMessage("2058557339")
    .AddTextMessage("请输入文本");

// 示例图片路径：将图片文件放到输出目录并命名为 image.png，或修改此路径
var imagePath = Path.Combine(AppContext.BaseDirectory, "image.png");
if (File.Exists(imagePath))
{
    using var fs = File.OpenRead(imagePath);
    builder.AddImageMessage(fs);
}
else
{
    Console.WriteLine($"未找到图片文件: {imagePath}，将仅发送文本消息。");
}

var message = builder.Build();

var res  = await bot.SendGroupMessageAsync(new GroupMessageSend()
{
    GroupId = "769372512",
    Message = message
});

Console.WriteLine(res.MessageId);
try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (TaskCanceledException)
{
}
