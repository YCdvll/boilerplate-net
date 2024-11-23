using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Project.Common.DataAccess.Models.Chat;

namespace Project.WebApi.Services;

public interface IWebSocketService
{
    Task HandleWebSocketAsync(WebSocket ws, int chatId, CancellationToken cancellationToken);
}

public class WebSocketService : IWebSocketService
{
    private readonly List<WebSocketChannel> _channels = new();

    public async Task HandleWebSocketAsync(WebSocket ws, int chatId, CancellationToken cancellationToken)
    {
        SubscribeChannel(chatId, ws);

        async void HandleMessage(WebSocketReceiveResult result, byte[] buffer)
        {
            if (result.MessageType == WebSocketMessageType.Text)
            {
                var jsonString = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var message = JsonSerializer.Deserialize<WebSocketMessage>(jsonString);

                await Broadcast(message, cancellationToken);
            }
            else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
            {
                foreach (var channel in _channels.ToList())
                {
                    if (channel.Connections.Any(x => x == ws))
                        channel.Connections.Remove(ws);

                    if (channel.Connections.Count == 0)
                        _channels.Remove(channel);
                }
            }
        }

        await ReceiveMessage(ws, HandleMessage, cancellationToken);
    }

    private void SubscribeChannel(int chatId, WebSocket ws)
    {
        if (_channels.Count == 0)
        {
            _channels.Add(new WebSocketChannel
            {
                ChannelId = chatId,
                Connections = [ws]
            });
        }
        else if (_channels.Any(x => x.ChannelId != chatId))
        {
            _channels.Add(new WebSocketChannel
            {
                ChannelId = chatId,
                Connections = [ws]
            });
        }
        else
        {
            var channel = _channels.FirstOrDefault(x => x.ChannelId == chatId);
            if (channel.Connections.All(x => x != ws))
                channel.Connections.Add(ws);
        }
    }

    private async Task Broadcast(WebSocketMessage message, CancellationToken cancellationToken)
    {
        var responseJson = JsonSerializer.Serialize(message);
        var bytes = Encoding.UTF8.GetBytes(responseJson);

        var channels = _channels.FirstOrDefault(x => x.ChannelId == message.ChatId)?.Connections;

        foreach (var socket in channels)
            if (socket.State == WebSocketState.Open)
            {
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, cancellationToken);
            }
    }

    private async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage, CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);
            handleMessage(result, buffer);
        }
    }
}

public class WebSocketChannel
{
    public int ChannelId { get; set; }
    public List<WebSocket> Connections { get; set; }
}