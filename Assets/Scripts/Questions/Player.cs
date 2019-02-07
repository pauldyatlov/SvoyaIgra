using System;
using System.Net.Sockets;

public class Player
{
    public string Name;
    public int Points;

    public NetworkStream Stream;

    public Action<Player> OnPointsUpdateAction;

    public Player(string name, NetworkStream stream)
    {
        Name = name;
        Points = 0;

        Stream = stream;
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
}