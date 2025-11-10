using System.Text.Json.Serialization;
using NapPlana.Core.Data.Message;
using NapPlana.Core.Utilities;

namespace NapPlana.Core.Data.API;

public class GroupMessageSend
{
    [JsonPropertyName("group_id")]
    public string GroupId { get; set; } = string.Empty;
    
    [JsonPropertyName("message")]
    [JsonConverter(typeof(MessageListConverter))]
    public List<MessageBase> Message { get; set; } = new();
}

public class GroupMessageSendResponseData : ResponseDataBase
{
    [JsonPropertyName("message_id")]
    public long MessageId { get; set; } = 0;
}

public class PokeMessageSend
{
    [JsonPropertyName("group_id")]
    public string? GroupId { get; set; } = null;
    
    [JsonPropertyName("user_id")]
    public string UserId { get; set; } = string.Empty;
    
    [JsonPropertyName("target_id")]
    public string TargetId { get; set; } = string.Empty;
}