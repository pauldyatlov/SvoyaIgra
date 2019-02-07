using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

public class QuizCommand
{
    public string Command;
    public string Parameter;
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
        _listener = new TcpListener(IPAddress.Any, 8888);
        _listener.Start();

        while (true)
            _ = HandleClient(await _listener.AcceptTcpClientAsync());
    }

    private static async Task HandleClient(TcpClient client)
    {
        try
        {
            Debug.Log("Client connected");

            using (client)
            {
                var stream = client.GetStream();

                while (true)
                    HandleAnswer(await stream.ReadString(), stream);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    private static void HandleAnswer(string json, NetworkStream stream)
    {
        if (string.IsNullOrEmpty(json))
        {
            var player = Engine.RegisteredPlayers.FirstOrDefault(x => x.Stream == stream);

            if (player == null)
                throw new Exception("[!] Unknown player disconnected");

            OnPlayerDisconnected?.Invoke(player);

            throw new Exception($"[!] {player.Name} disconnected");
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

                OnPlayerConnected?.Invoke(new Player(action.Parameter, stream));

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