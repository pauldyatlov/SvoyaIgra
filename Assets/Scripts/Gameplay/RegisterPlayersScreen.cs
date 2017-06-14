using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RegisterPlayersScreen : MonoBehaviour
{
    [SerializeField] private InputField _playerName;
    [SerializeField] private Button _addPlayerButton;
    [SerializeField] private Button _playButton;

    [SerializeField] private GamePlayer _playerEntityTemplate;
    [SerializeField] private RectTransform _playersList;

    public List<Player> _activePlayers = new List<Player>();
    private Action<List<Player>> _onPlayButtonPressed;

    private void Awake()
    {
        _addPlayerButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitForInput());
        });

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

    private IEnumerator WaitForInput()
    {
        while (true)
        {
            foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKey(vKey))
                {
                    var player = new Player(_playerName.text, vKey);
                    _activePlayers.Add(player);

                    var instantiatedPlayerEntity = Instantiate(_playerEntityTemplate);
                    instantiatedPlayerEntity.Init(player, OnPlayerDeletedHandler);

                    instantiatedPlayerEntity.transform.SetParent(_playersList, false);
                    Debug.Log("Player added. Name: " + _playerName.text + " key: " + vKey);

                    _playerName.text = "";

                    yield break;
                }
            }

            yield return null;
        }
    }

    private void OnPlayerDeletedHandler(GamePlayer gamePlayer)
    {
        _activePlayers.Remove(gamePlayer.Player);

        Destroy(gamePlayer.gameObject);
    }
}