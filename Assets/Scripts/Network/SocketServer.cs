using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class QuizCommand
{
    public string Command;
    public string Parameter;
}

public static class SocketServer
{
    private const string PerformConnect = "PerformConnect";
    private const string ConnectApproved = "ConnectApproved";

    private const string MakeAnswer = "MakeAnswer";

    public const string CorrectAnswer = "CorrectAnswer";
    public const string WrongAnswer = "WrongAnswer";

    public static Action<Player> OnPlayerConnected;

    public static Action<string> OnPlayerAnswered;
    private static TcpListener _listener;

    public static async void Init()
    {
        _listener = new TcpListener(IPAddress.Any, 8888);
        _listener.Start();

        while (true)
            HandleClient(await _listener.AcceptTcpClientAsync());
    }

    private static async void HandleClient(TcpClient client)
    {
        Debug.Log("Client connected");

        using (client)
        {
            var stream = client.GetStream();

            while (true)
                HandleAnswer(await stream.ReadString(), client);
        }
    }

    private static void HandleAnswer(string json, TcpClient client)
    {
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogError("Its null");

            return;
        }

        var action = JsonUtility.FromJson<QuizCommand>(json);

        if (action == null)
        {
            Debug.LogError("Something went wrong");

            return;

            SendMessage(client, new QuizCommand
                {
                    Command = "Disconnect",
                    Parameter = ":("
                },
                () => { });

            return;
        }

        Debug.Log("Command received: " + action.Command);

        if (action.Command == PerformConnect)
        {
            Debug.Log("New player connected: " + action.Parameter);

            OnPlayerConnected?.Invoke(new Player(action.Parameter, client));

//            var firstOrDefault = Engine.RegisteredPlayers.FirstOrDefault(x => x.Name == action.Parameter);

            SendMessage(client, new QuizCommand
            {
                Command = ConnectApproved,
                Parameter = action.Parameter,
            });
        }
        else if (action.Command == MakeAnswer)
        {
            OnPlayerAnswered?.Invoke(action.Parameter);
        }
    }

    public static async void SendMessage(TcpClient client, QuizCommand message, Action callback = null)
    {
        var stream = client.GetStream();

        await stream.WriteString(JsonUtility.ToJson(message));

        callback?.Invoke();
    }
}