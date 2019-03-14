using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class QuizCommand
{
    public string Command;
    public string Parameter;
}

public interface IMatchmakerServer
{

}

public class Matchmaker
{
    private class Response
    {
        public class Message
        {
            public string id;
            public string text;
        }

        public class PlayerState
        {
            public string id;
            public bool online;
        }

        public string PlayerConnected;
        public string PlayerDisconnected;
        public Message PlayerMessage;
        public PlayerState[] CurrentState;
    }

    public class Player
    {
        public readonly string Id;
        public bool Online { get; private set; }

        private readonly Matchmaker _matchmaker;

        internal Player(string id, bool online, Matchmaker matchmaker)
        {
            Id = id;
            Online = online;
            _matchmaker = matchmaker;
        }

        public Action<string> MessageReceived;

        public void SendMessage(string data)
        {
            _matchmaker.SendMessage(Enumerable.Repeat(this, 1), data);
        }
    }

    public readonly List<Player> Players = new List<Player>();
    public event Action<Player> PlayerAdded;
    public event Action<Player> PlayerRemoved;

    private readonly TcpClient _tcpClient;
    private NetworkStream _networkStream;

    private Matchmaker(TcpClient tcpClient)
    {
        _tcpClient = tcpClient;
    }

    public static async Task<Matchmaker> Create(string host, int port)
    {
        var tcpClient = new TcpClient();
        await tcpClient.ConnectAsync(host, port);
//        tcpClient.Connect(host, port);

        var matchmaker = new Matchmaker(tcpClient);
        _ = matchmaker.MainLoop();
        return matchmaker;
    }

    private void SendToIds(ICollection<string> ids, string message)
    {
        Debug.LogWarning("Send to ids: " + string.Join(", ", ids) + ": " + message);

        async void Send()
        {
            try {
                await _networkStream.WriteString(JsonConvert.SerializeObject(new {
                    Message = new {
                        target = ids.Count == 1
                            ? (object)new { Id = ids.Single() }
                            : (object)new { Ids = ids },
                        message
                    }
                }));
            } catch (Exception e) {
                Debug.LogException(e);
            }
        }

        Send();
    }

    public void SendMessage(IEnumerable<Player> players, string message)
    {
        var onlinePlayers = players.Where(x => x.Online).ToArray();

        if (onlinePlayers.Any())
            SendToIds(onlinePlayers.Select(x => x.Id).ToArray(), message);
    }

    public void Broadcast(string message) =>
        _ = _networkStream.WriteString(JsonConvert.SerializeObject(new {
            Message = new {
                target = "Broadcast",
                message
            }
        }));

    private async Task MainLoop()
    {
        using (_networkStream = _tcpClient.GetStream()) {
            await _networkStream.WriteString(@"{ ""Login"": ""SVOYAIGRA"" }");

            while (true)
            {
                try {
                    var matchmakerMessage = await _networkStream.ReadString();

                    Debug.LogWarning("Message: " + matchmakerMessage);

                    var response = JsonConvert.DeserializeObject<Response>(matchmakerMessage);

                    if (response == null)
                        throw new IOException("Oh shit");

                    if (response.CurrentState != null) {
                        foreach (var playerState in response.CurrentState) {
                            var player = new Player(playerState.id, playerState.online, this);
                            Players.Add(player);
                            PlayerAdded?.Invoke(player);
                        }
                    } else if (!string.IsNullOrEmpty(response.PlayerConnected)) {
                        var player = new Player(response.PlayerConnected, true, this);
                        Players.Add(player);
                        PlayerAdded?.Invoke(player);
                    } else if (!string.IsNullOrEmpty(response.PlayerDisconnected))
                        Players.RemoveAll(x => x.Id == response.PlayerDisconnected);
                    else if (response.PlayerMessage != null) {
                        var innerMessage = response.PlayerMessage.text.Replace("\\\"", "\"");
                        Debug.LogWarning("Inner message: " + innerMessage);
                        Players.Single(x => x.Id == response.PlayerMessage.id).MessageReceived?.Invoke(innerMessage);
                    }
                } catch (IOException ioException) {
                    Debug.LogError("Matchmaker has been disconnected: " + ioException.Message);
                    throw;
                } catch (Exception e) {
                    Debug.LogException(e);
                }
            }
        }
    }
}

public static class SocketServer
{
    public const string PerformConnect = "PerformConnect";
    public const string ConnectApproved = "ConnectApproved";
    public const string MakeAnswer = "MakeAnswer";
    public const string CorrectAnswer = "CorrectAnswer";
    public const string WrongAnswer = "WrongAnswer";
    public const string SetScore = "SetPoints";

    public static Action<Player> OnPlayerConnected;
    public static Action<Player> OnPlayerDisconnected;
    public static Action<string> OnPlayerAnswered;

    private static TcpListener _listener;

    public static async void Init()
    {
        var matchmaker = await Matchmaker.Create("139.162.199.78", 3013);
        //var matchmaker = await Matchmaker.Create("127.0.0.1", 3013);

        var players = matchmaker.Players.Select(x => new Player(x)).ToArray();

        matchmaker.PlayerAdded += player => _ = new Player(player);




        return;

        var tcpClient = new TcpClient();

        tcpClient.Connect("localhost", 3013);

        var stream = tcpClient.GetStream();
        _ = HandleMatchmaker(stream);

        await stream.WriteString(JsonUtility.ToJson(new { Login = (object)null }));
        await Task.Delay(TimeSpan.FromSeconds(5));
        await stream.WriteString("SUKO BIATCH");

        _listener = new TcpListener(IPAddress.Any, 8888);
        _listener.Start();

        while (true)
            _ = HandleClient(await _listener.AcceptTcpClientAsync());
    }

    private static async Task HandleMatchmaker(NetworkStream stream)
    {
        while (true)
        {
            var matchmakerMessage = await stream.ReadString();

        }
    }

    private static async Task HandleClient(TcpClient client)
    {
        try {
            Debug.Log("Client connected");

            using (client) {
                var stream = client.GetStream();

                while (true)
                    HandleAnswer(await stream.ReadString(), stream);
            }
        } catch (Exception e) {
            Debug.LogError(e);
        }
    }

    private static void HandleAnswer(string json, NetworkStream stream)
    {
        if (string.IsNullOrEmpty(json))
        {
//            var player = Engine.RegisteredPlayers.FirstOrDefault(x => x.Stream == stream);

//            if (player == null)
//                throw new Exception("[!] Unknown player disconnected");

//            OnPlayerDisconnected?.Invoke(player);
//
//            throw new Exception($"[!] {player.Name} disconnected");
        }

        var action = JsonUtility.FromJson<QuizCommand>(json);
        if (action == null)
            throw new Exception("Action was not handled: " + action);

        Debug.Log("Command received: " + action.Command);

        switch (action.Command)
        {
            case PerformConnect:
            {
                Debug.Log("New player connected: " + action.Parameter);

//                OnPlayerConnected?.Invoke(new Player(action.Parameter, stream));

//            var firstOrDefault = Engine.RegisteredPlayers.FirstOrDefault(x => x.Name == action.Parameter);

                SendMessage(stream, new QuizCommand
                {
                    Command = ConnectApproved,
                    Parameter = action.Parameter,
                });
                break;
            }
            case MakeAnswer:
            {
                OnPlayerAnswered?.Invoke(action.Parameter);
                break;
            }
        }
    }

    public static async void SendMessage(NetworkStream stream, QuizCommand message, Action callback = null)
    {
        await stream.WriteString(JsonUtility.ToJson(message));

        callback?.Invoke();
    }
}