using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TaskEntity : MonoBehaviour
{
    [SerializeField] private Text _answeringPlayerLabel;
    [SerializeField] private Text _timerLabel;
    [SerializeField] private Text _label;
    [SerializeField] private Image _image;
    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;

    private QuestionsGameplayPlan _gameplayPlan;
    private Player _answeringPlayer;

    private int _maxTimer = 10;

    private void Awake()
    {
        _acceptButton.onClick.AddListener(CorrectAnswer);
        _declineButton.onClick.AddListener(IncorrectAnswer);
    }

    public void Show(QuestionsGameplayPlan plan)
    {
        gameObject.SetActive(true);
        _acceptButton.gameObject.SetActive(false);
        _declineButton.gameObject.SetActive(false);

        _gameplayPlan = plan;
        _label.text = _gameplayPlan.Question;

        if (plan.Picture != null) {
            _image.sprite = plan.Picture;
        }

        _image.gameObject.SetActive(plan.Picture != null);

        var fullTime = _maxTimer + (_gameplayPlan.Question.Length * .04f);

        _answeringPlayerLabel.text = "";
        _timerLabel.text = fullTime.ToString(CultureInfo.InvariantCulture);

        StartCoroutine(Co_GameplayRoundTimer(fullTime));
        _answeringPlayer = null;

        Engine.OnPlayerKeycodePressed += arg =>
        {
            if (_answeringPlayer == null)
            {
                _acceptButton.gameObject.SetActive(true);
                _declineButton.gameObject.SetActive(true);

                Time.timeScale = 0.0f;

                _answeringPlayerLabel.text = "ОТВЕЧАЕТ " + arg.Name;
                _answeringPlayer = arg;
            }
        };
    }

    public void CorrectAnswer()
    {
        _answeringPlayerLabel.text = "";
        gameObject.SetActive(false);

        _answeringPlayer.UpdatePoints(_gameplayPlan.Price);

        Time.timeScale = 1.0f;
    }

    public void IncorrectAnswer()
    {
        _answeringPlayerLabel.text = "";

        _answeringPlayer.UpdatePoints(-_gameplayPlan.Price);

        Time.timeScale = 1.0f;

        _acceptButton.gameObject.SetActive(false);
        _declineButton.gameObject.SetActive(false);

        _answeringPlayer = null;
    }

    private IEnumerator Co_GameplayRoundTimer(float time)
    {
        while (true)
        {
            for (var i = 0; i < time; i++)
            {
                yield return new WaitForSeconds(1);

                _timerLabel.text = (time - i).ToString(CultureInfo.InvariantCulture);
            }

            gameObject.SetActive(false);
        }
    }
}