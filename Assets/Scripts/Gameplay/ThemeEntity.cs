using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ThemeEntity : MonoBehaviour
{
    [SerializeField] private QuestionEntity _questionTemplate;
    [SerializeField] private Text _themeNameLabel;
    [SerializeField] private List<RectTransform> _questionsPlaceholders;
    [SerializeField] private TaskEntity _taskEntity;

    private readonly List<QuestionEntity> _availableQuestions = new List<QuestionEntity>();

    public Action<ThemeEntity, int> _onAvailableQuestionsEnd;

    public void Init(RectTransform placeholder, List<QuestionsGameplayPlan> questions, string themeName, List<Player> playersList, int round)
    {
        gameObject.SetActive(true);

        transform.SetParent(placeholder, false);

        _themeNameLabel.text = themeName;

        for (var i = 0; i < questions.Count; i++)
        {
            var index = i;
            var instantiatedQuestion = Instantiate(_questionTemplate);

            _availableQuestions.Add(instantiatedQuestion);
            instantiatedQuestion.Init(questions[index].Price.ToString(), _questionsPlaceholders[index], () =>
            {
                _availableQuestions.Remove(instantiatedQuestion);

                if (_availableQuestions.Count <= 0 && _onAvailableQuestionsEnd != null)
                {
                    _onAvailableQuestionsEnd(this, round);
                }

                instantiatedQuestion.gameObject.SetActive(false);

                _taskEntity.Show(questions[index]);
            });
        }
    }
}