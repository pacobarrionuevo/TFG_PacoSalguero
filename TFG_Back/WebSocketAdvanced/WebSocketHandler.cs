﻿using System.IO;
using System.Net.WebSockets;
using System.Text;

namespace TFG_Back.WebSocketAdvanced
{
    public class WebSocketHandler : IDisposable
    {
        private const int BufferSize = 4096;
        private readonly WebSocket _webSocket;
        private readonly byte[] _buffer;
        public string Username { get; set; }

        public DateTime LastActivity { get; set; }
        public int Id { get; init; }
        public bool IsOpen => _webSocket.State == WebSocketState.Open;

        public event Func<WebSocketHandler, string, Task> MessageReceived;
        public event Func<WebSocketHandler, Task> Disconnected;
        public string MessageType { get; set; }
        public WebSocketHandler(int userId, WebSocket webSocket, string username)
        {
            Id = userId;
            _webSocket = webSocket;
            Username = username;
            LastActivity = DateTime.UtcNow;
            _buffer = new byte[BufferSize];
        }
        public async Task HandleAsync()
        {
            while (_webSocket.State == WebSocketState.Open)
            {
                try
                {
                    string message = await ReadAsync();
                    LastActivity = DateTime.UtcNow;

                    if (!string.IsNullOrWhiteSpace(message) && MessageReceived != null)
                    {
                        await MessageReceived.Invoke(this, message);
                    }
                }
                catch (WebSocketException)
                {
                    Dispose();
                }
            }

            if (Disconnected != null)
            {
                await Disconnected.Invoke(this);
            }
        }
        private async Task<string> ReadAsync()
        {
            using var memoryStream = new MemoryStream();
            WebSocketReceiveResult receiveResult;

            do
            {
                receiveResult = await _webSocket.ReceiveAsync(_buffer, CancellationToken.None);
                if (receiveResult.MessageType == WebSocketMessageType.Text)
                {
                    memoryStream.Write(_buffer, 0, receiveResult.Count);
                }
                else if (receiveResult.CloseStatus.HasValue)
                {
                    await _webSocket.CloseAsync(receiveResult.CloseStatus.Value, receiveResult.CloseStatusDescription, CancellationToken.None);
                }
            } while (!receiveResult.EndOfMessage);

            return Encoding.UTF8.GetString(memoryStream.ToArray());
        }

        public async Task SendAsync(string message)
        {
            if (IsOpen)
            {
                byte[] bytes = Encoding.UTF8.GetBytes(message);
                await _webSocket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        public void Dispose()
        {
            _webSocket.Dispose();
        }

        private async Task<string> ReceiveMessageAsync()
        {
            var buffer = new byte[1024];
            WebSocketReceiveResult result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            return Encoding.UTF8.GetString(buffer, 0, result.Count);
        }

        public async Task CloseAsync()
        {
            await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Conexión cerrada", CancellationToken.None);
        }
    }
}