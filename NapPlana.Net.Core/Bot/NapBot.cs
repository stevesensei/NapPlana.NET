using NapPlana.Core.API;
using NapPlana.Core.Connections;
using NapPlana.Core.Data;
using NapPlana.Core.Data.API;

namespace NapPlana.Core.Bot;

public class NapBot
{
    private ConnectionBase _connection;
    
    public NapBot()
    {
        // Default to a dummy connection; should be set properly later
        _connection = new ConnectionBase();
    }

    // Added: constructor that accepts a connection
    public NapBot(ConnectionBase connection)
    {
        _connection = connection;
    }

    // Added: fluent setter for the connection
    public NapBot SetConnection(ConnectionBase connection)
    {
        _connection = connection;
        return this;
    }

    // Added: lifecycle helpers
    public Task StartAsync() => _connection.InitializeAsync();
    public Task StopAsync() => _connection.ShutdownAsync();
    
    public async Task<GroupMessageSendResponseData> SendGroupMessageAsync(GroupMessageSend groupMessage)
    {
        if (groupMessage is null) throw new ArgumentNullException(nameof(groupMessage));
        
        var echo = Guid.NewGuid().ToString();
        await _connection.SendMessageAsync(ApiActionType.SendGroupMsg, groupMessage, echo);
        
        var timeout = TimeSpan.FromSeconds(15);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (ApiHandler.TryConsume(echo, out var response))
            {
                if (response.RetCode != 0)
                {
                    throw new InvalidOperationException($"send_group_msg failed: {response.RetCode} - {response.Message}");
                }
                var data = response.GetData<GroupMessageSendResponseData>();
                if (data != null)
                {
                    return data;
                }
                throw new InvalidOperationException("Failed to parse send_group_msg response data.");
            }

            await Task.Delay(50);
        }

        throw new TimeoutException("Timed out waiting for send_group_msg response.");
    }
    
    public async Task SendPokeAsync(PokeMessageSend pokeMessage)
    {
        if (pokeMessage is null) throw new ArgumentNullException(nameof(pokeMessage));
        
        var echo = Guid.NewGuid().ToString();
        await _connection.SendMessageAsync(ApiActionType.SendPoke, pokeMessage, echo);
        
        var timeout = TimeSpan.FromSeconds(15);
        var start = DateTime.UtcNow;

        while (DateTime.UtcNow - start < timeout)
        {
            if (ApiHandler.TryConsume(echo, out var response))
            {
                if (response.RetCode != 0)
                {
                    throw new InvalidOperationException($"send_poke failed: {response.RetCode} - {response.Message}");
                }
                return;
            }

            await Task.Delay(50);
        }

        throw new TimeoutException("Timed out waiting for send_poke response.");
    }
}