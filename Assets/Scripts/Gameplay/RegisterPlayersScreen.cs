using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class RegisterPlayersScreen : MonoBehaviour
{
    [SerializeField] private Button _playButton;

    [SerializeField] private GamePlayer _playerEntityTemplate;
    [SerializeField] private RectTransform _playersList;

    private readonly List<Player> _activePlayers = new List<Player>();
    private Action<List<Player>> _onPlayButtonPressed;

    private void Awake()
    {
        _playButton.onClick.AddListener(() =>
        {
            if (_onPlayButtonPressed == null) return;

            gameObject.SetActive(false);
            _onPlayButtonPressed.Invoke(_activePlayers);
        });
    }

    public void Init(Action<List<Player>> playButtonPressed)
    {
        _onPlayButtonPressed = playButtonPressed;

        gameObject.SetActive(true);
    }

    private void NewPlayerConnectedHandler(Player player)
    {
        _activePlayers.Add(player);

        var entity = Instantiate(_playerEntityTemplate);

        entity.Init(player, OnPlayerDeletedHandler);
        entity.transform.SetParent(_playersList, false);
    }

    private void OnPlayerDeletedHandler(GamePlayer gamePlayer)
    {
        _activePlayers.Remove(gamePlayer.Player);

        Destroy(gamePlayer.gameObject);
    }
}