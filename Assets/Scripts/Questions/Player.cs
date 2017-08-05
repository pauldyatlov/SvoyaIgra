using System;
using System.Net.Sockets;

public class Player
{
    public string Name;
    public int Points;

    public TcpClient TcpClient;

    public Action<Player> OnPointsUpdateAction;

    public Player(string name, TcpClient client)
    {
        Name = name;
        Points = 0;

        TcpClient = client;
    }

    public void UpdatePoints(int arg)
    {
        Points += arg;

        OnPointsUpdateAction?.Invoke(this);
    }
}