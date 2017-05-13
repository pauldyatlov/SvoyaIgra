using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameTheme : MonoBehaviour
{
    [SerializeField] private GameQuestion _gameQuestionTemplate;
    [SerializeField] private Text _themeNameLabel;
    [SerializeField] private List<RectTransform> _questionsPlaceholders;
    [SerializeField] private TaskScreen _taskScreen;

    private readonly List<GameQuestion> _availableQuestions = new List<GameQuestion>();

    public Action<GameTheme, int> _onAvailableQuestionsEnd;

    public void Init(RectTransform placeholder, List<QuestionsGameplayPlan> questions, string themeName, List<Player> playersList, int round)
    {
        gameObject.SetActive(true);

        transform.SetParent(placeholder, false);

        _themeNameLabel.text = themeName;

        for (var i = 0; i < questions.Count; i++)
        {
            var index = i;
            var instantiatedQuestion = Instantiate(_gameQuestionTemplate);

            _availableQuestions.Add(instantiatedQuestion);
            instantiatedQuestion.Init(questions[index].Price.ToString(), _questionsPlaceholders[index], () =>
            {
                _availableQuestions.Remove(instantiatedQuestion);

                if (_availableQuestions.Count <= 0 && _onAvailableQuestionsEnd != null)
                {
                    _onAvailableQuestionsEnd(this, round);
                }

                instantiatedQuestion.gameObject.SetActive(false);

                _taskScreen.Show(questions[index]);
            });
        }
    }
}