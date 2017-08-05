using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class QuizCommand
{
    public string Command;
    public string Parameter;
    public string SecondParameter;
}

public static class SocketServer
{
    private const string PerformConnect = "PerformConnect";
    private const string ConnectApproved = "ConnectApproved";

    private const string MakeAnswer = "MakeAnswer";

    public const string CorrectAnswer = "CorrectAnswer";
    public const string WrongAnswer = "WrongAnswer";

    public static Action<Player> OnPlayerConnected;
    public static Action<TcpClient> OnPlayerDisconnected;

    public static Action<string> OnPlayerAnswered;

    public static async void Init()
    {
        var listener = new TcpListener(IPAddress.Any, 8888);

        listener.Start();

        while (true)
        {
            HandleClient(await listener.AcceptTcpClientAsync());
        }
    }

    private static async void HandleClient(TcpClient client)
    {
        Debug.Log("Client connected");

        using (client)
        {
            var stream = client.GetStream();

            while (true)
            {
                HandleAnswer(await stream.ReadString(), client);
            }
        }
    }

    private static void HandleAnswer(string json, TcpClient client)
    {
        try
        {
            var action = JsonUtility.FromJson<QuizCommand>(json);

            if (action == null)
            {
                Debug.LogError("Something went wrong");

                OnPlayerDisconnected(client);

                SendMessage(client, new QuizCommand
                {
                    Command = "Disconnect",
                    Parameter = ":("
                },
                    client.Dispose);

                return;
            }

            Debug.Log("Command received: " + action.Command);

            if (action.Command == PerformConnect)
            {
                Debug.Log("New player connected: " + action.Parameter);

                OnPlayerConnected?.Invoke(new Player(action.Parameter, client));

                var firstOrDefault = Engine.RegisteredPlayers.FirstOrDefault(x => x.Name == action.Parameter);

                SendMessage(client, new QuizCommand
                {
                    Command = ConnectApproved,
                    Parameter = action.Parameter,
                    SecondParameter = firstOrDefault?.Points.ToString() ?? "0"
                });
            }
            else if (action.Command == MakeAnswer)
            {
                OnPlayerAnswered?.Invoke(action.Parameter);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error happened, but it's okay: " + e);
        }
    }

    public static async void SendMessage(TcpClient client, QuizCommand message, Action callback = null)
    {
        var stream = client.GetStream();

        await stream.WriteString(JsonUtility.ToJson(message));

        callback?.Invoke();
    }
}