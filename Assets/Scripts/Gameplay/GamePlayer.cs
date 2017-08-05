﻿using System;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    [SerializeField] private Text _playerLabel;
    [SerializeField] private Button _deletePlayerButton;

    private Action<GamePlayer> _onPlayerDeletedAction;
    public Player Player;

    private void Awake()
    {
        _deletePlayerButton.onClick.AddListener(() =>
        {
            _onPlayerDeletedAction(this);
        });
    }

    public void Init(Player player, Action<GamePlayer> onPlayerDeletedAction)
    {
        Player = player;
        _onPlayerDeletedAction = onPlayerDeletedAction;

        _playerLabel.text = "NAME: " + player.Name;
    }
}