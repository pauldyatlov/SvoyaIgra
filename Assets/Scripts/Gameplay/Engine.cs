using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Engine : MonoBehaviour
{
    [SerializeField] private AddPlayers _addPlayers;

    [SerializeField] private ThemeEntity _themeTemplate;
    [SerializeField] private RectTransform _placeholder;

    [SerializeField] private GameplayPlan _gameplayPlan;

    [SerializeField] private PlayerStatsEntity _playerStatsTemplate;
    [SerializeField] private RectTransform _playerStatsPlaceholder;

    private int _currentRound;

    private readonly Dictionary<ThemeEntity, int> _themesGameplayPlans = new Dictionary<ThemeEntity, int>();

    public static Action<Player> OnPlayerKeycodePressed;

    private void Awake()
    {
        _addPlayers.Init(arg =>
        {
            foreach (var player in arg)
            {
                var stat = Instantiate(_playerStatsTemplate);

                stat.transform.SetParent(_playerStatsPlaceholder, false);
                stat.Init(player);

                player.OnPointsUpdateAction(player);
            }

            for (var i = 0; i < _gameplayPlan.RoundsList.Count; i++)
            {
                var plan = _gameplayPlan.RoundsList[i];

                foreach (var theme in plan.ThemesList)
                {
                    var createdTheme = Instantiate(_themeTemplate);
                    
                    _themesGameplayPlans.Add(createdTheme, i);

                    createdTheme.Init(_placeholder, theme.QuestionsList, theme.ThemeName, arg, i);
                    createdTheme._onAvailableQuestionsEnd += OnAvailableQuestionsEndHandler;

                    if (i != 0)
                    {
                        createdTheme.gameObject.SetActive(false);
                    }

                    StartCoroutine(Co_WaitForInput(arg));
                }
            }
        });
    }

    private void OnAvailableQuestionsEndHandler(ThemeEntity entity, int round)
    {
        _themesGameplayPlans.Remove(entity);

        entity.gameObject.SetActive(false);

        if (_placeholder.Cast<Transform>().Any(child => child.gameObject.activeSelf))
        {
            return;
        }

        _currentRound++;

        for (var i = 0; i < _themesGameplayPlans.Keys.Count; i++)
        {
            if (_themesGameplayPlans.Values.ElementAt(i) == _currentRound)
            {
                _themesGameplayPlans.Keys.ElementAt(i).gameObject.SetActive(true);
            }
        }
    }

    private static IEnumerator Co_WaitForInput(List<Player> players)
    {
        while (true)
        {
            foreach (var player in players.Where(player => Input.GetKeyDown(player.Code)).Where(player => OnPlayerKeycodePressed != null))
            {
                OnPlayerKeycodePressed(player);
            }

            yield return null;
        }
    }
}