using System;
using UnityEngine;

public class Player
{
    public int Points;
    public string Name;
    public KeyCode Code;

    public Action<Player> OnPointsUpdateAction;

    public Player(string name, KeyCode code)
    {
        Name = name;
        Code = code;
    }

    public void UpdatePoints(int arg)
    {
        Points += arg;

        if (OnPointsUpdateAction != null)
        {
            OnPointsUpdateAction(this);
        }
    }
}