using System;
using Newtonsoft.Json;

public class Player
{
    public string Name;
    public int Points;

    public Matchmaker.Player Stream;

    public Action<Player> OnPointsUpdateAction;
    public Action<string> OnNameChanged;

    public class PlayerMessage
    {
        public string SetName;
        public string Answer;
    }

    public Player(Matchmaker.Player stream)
    {
        Points = 0;
        Name = $"Player {stream.Id}";

        stream.MessageReceived += text =>
        {
            var message = JsonConvert.DeserializeObject<PlayerMessage>(text);

            if (message.SetName != null) {
                Name = message.SetName;
                OnNameChanged?.Invoke(Name);
            } else if (message.Answer != null)
                SocketServer.OnPlayerAnswered?.Invoke(Name);
        };

        Stream = stream;

        SocketServer.OnPlayerConnected?.Invoke(this);
    }

    public void UpdatePoints(int arg)
    {
        Points += arg;
        OnPointsUpdateAction?.Invoke(this);
    }

    public void SetPoints(int arg)
    {
        Points = arg;
        OnPointsUpdateAction?.Invoke(this);
    }

    public void SendMessage(QuizCommand quizCommand)
    {
        Stream.SendMessage(JsonConvert.SerializeObject(quizCommand));
    }
}