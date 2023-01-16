using Devameet_CSharp.Repository;
using System.Net.WebSockets;
using System.Text;

namespace Devameet_CSharp.Controllers
{
    public class WebSocketController
    {
        private readonly RequestDelegate _next;
        public readonly IRoomRepository _roomRepository;

        public WebSocketController(RequestDelegate next, IRoomRepository roomRepository)
        {
            _next = next;
            _roomRepository = roomRepository;
        }

        public async Task Invoke(HttpContext context)
        {
            // Se não for uma solicitação de socket tem que fechar a conexao
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next.Invoke(context);
                return;
            }

            // Verifica o que está sendo enviado se for uma solicitação de socket 
            var ct = context.RequestAborted;
            using (var socket = await context.WebSockets.AcceptWebSocketAsync())
            {
                var msg = await ReceiveStringAsync(socket, ct);
                if (msg == null) return;

                // Processa a msg
                await SendStringAsync(socket, _roomRepository.GetRoom(msg), ct);

                return;
            }
        }

        private static async Task<string> ReceiveStringAsync(WebSocket socket, CancellationToken ct = default)
        {
            // Se recibe un mensaje que debe ser descodificado
            var buffer = new ArraySegment<byte>(new byte[8192]);
            using (var ms = new MemoryStream())
            {
                WebSocketReceiveResult result;
                do
                {
                    ct.ThrowIfCancellationRequested();

                    result = await socket.ReceiveAsync(buffer, ct);
                    ms.Write(buffer.Array, buffer.Offset, result.Count);
                }
                while (!result.EndOfMessage);

                ms.Seek(0, SeekOrigin.Begin);
                if (result.MessageType != WebSocketMessageType.Text)
                    throw new Exception("Mensaje inesperado");

                // Codificar como UTF8: https://tools.ietf.org/html/rfc6455#section-5.6
                using (var reader = new StreamReader(ms, Encoding.UTF8))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        private static Task SendStringAsync(WebSocket socket, string data, CancellationToken ct = default)
        {
            var buffer = Encoding.UTF8.GetBytes(data);
            var segment = new ArraySegment<byte>(buffer);
            return socket.SendAsync(segment, WebSocketMessageType.Text, true, ct);
        }

    }

}
