using Newtonsoft.Json;
using SimpleWS_Server;
using System;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Xml.Linq;

public class Program
{
    static List<WebSocket> connections = new List<WebSocket>();
    static List<string> names = new List<string>();

    public static void Main(string[] args)
    {
        Console.WriteLine("Staring Server..");
        SetUpRemoteReceiver(args);
        SetUpLocalServer(args);
        while(true)
        {

        }
    }

    static async void SetUpLocalServer(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://localhost:6969");

        var app = builder.Build();
        app.UseWebSockets();
        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var curName = context.Request.Query["name"];

                using var ws = await context.WebSockets.AcceptWebSocketAsync();

                connections.Add(ws);
                Console.WriteLine(curName + " Connected!");


              //  await Broadcast($"{curName} joined the room");
              // await Broadcast($"{connections.Count} users connected");
                await ReceiveMessage(ws,
                    async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            await Broadcast(curName + ": " + message);
                        }
                        else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                        {
                            connections.Remove(ws);
                            await Broadcast($"{curName} left the room");
                            await Broadcast($"{connections.Count} users connected");
                            await ws.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                            Console.WriteLine($"{curName} left the room");
                        }
                    });
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        });

        await app.RunAsync();
    }

    static async Task ReceiveMessage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handleMessage)
    {
        var buffer = new byte[1024 * 4];
        while (socket.State == WebSocketState.Open)
        {
            var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            handleMessage(result, buffer);
        }
    }

    static async Task Broadcast(string message)
    {
        Position pos = util.RandomPosition(message);
        string posString = JsonConvert.SerializeObject(pos);
        var bytes = Encoding.UTF8.GetBytes(posString);
        foreach (var socket in connections)
        {
            if (socket.State == WebSocketState.Open)
            {
                var arraySegment = new ArraySegment<byte>(bytes, 0, bytes.Length);
                await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);

                Console.WriteLine(message + "'s position Sent!");
            }
        }
    }

    static async void SetUpRemoteReceiver(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.WebHost.UseUrls("http://localhost:6979");

        var app = builder.Build();
        app.UseWebSockets();
        app.Map("/ws", async context =>
        {
            if (context.WebSockets.IsWebSocketRequest)
            {
                var curName = context.Request.Query["name"];

                using var ws = await context.WebSockets.AcceptWebSocketAsync();

                connections.Add(ws);
                Console.WriteLine(curName + " Connected!");

                await ReceiveMessage(ws,
                    async (result, buffer) =>
                    {
                        if (result.MessageType == WebSocketMessageType.Text)
                        {
                            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                            names.Add(message);
                            await Broadcast(curName + ": " + message);
                        }
                        else if (result.MessageType == WebSocketMessageType.Close || ws.State == WebSocketState.Aborted)
                        {
                          
                            Console.WriteLine($"{curName} left the room");
                        }
                    });
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        });

        await app.RunAsync();
    }
}


