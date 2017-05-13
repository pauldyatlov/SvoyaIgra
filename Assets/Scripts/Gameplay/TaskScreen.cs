using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class TaskScreen : MonoBehaviour
{
    [SerializeField] private Text _answeringPlayerLabel;
    [SerializeField] private Text _timerLabel;
    [SerializeField] private Text _label;
    [SerializeField] private Image _image;

    [SerializeField] private Button _acceptButton;
    [SerializeField] private Button _declineButton;

    [SerializeField] private Button _canAnswerButton;

    private QuestionsGameplayPlan _gameplayPlan;
    private Player _answeringPlayer;

    private const int MinTimer = 10;
    private bool _canAcceptAnswers;

    private void Awake()
    {
        _acceptButton.onClick.AddListener(CorrectAnswer);
        _declineButton.onClick.AddListener(IncorrectAnswer);

        _canAnswerButton.onClick.AddListener(CanAnswerHandler);
    }

    public void Show(QuestionsGameplayPlan plan)
    {
        gameObject.SetActive(true);

        _acceptButton.gameObject.SetActive(false);
        _declineButton.gameObject.SetActive(false);
        _canAnswerButton.gameObject.SetActive(true);

        _canAcceptAnswers = false;

        _gameplayPlan = plan;
        _label.text = _gameplayPlan.Question;

        if (plan.Picture != null) {
            _image.sprite = plan.Picture;
        }

        _image.gameObject.SetActive(plan.Picture != null);
        
        _answeringPlayerLabel.text = "";
        _answeringPlayer = null;

        _timerLabel.text = MinTimer.ToString(CultureInfo.InvariantCulture);

        Engine.OnPlayerKeycodePressed += arg =>
        {
            if (_canAcceptAnswers)
            {
                if (_answeringPlayer == null)
                {
                    _acceptButton.gameObject.SetActive(true);
                    _declineButton.gameObject.SetActive(true);

                    Time.timeScale = 0.0f;

                    _answeringPlayerLabel.text = "ОТВЕЧАЕТ " + arg.Name;
                    _answeringPlayer = arg;
                }
                else
                {
                    Debug.Log("Someone else is answering. tsshht!");
                }
            }
            else
            {
                Debug.Log("I can't accept answers now");
            }
        };
    }

    private void CanAnswerHandler()
    {
        StartCoroutine(Co_GameplayRoundTimer(MinTimer));

        _canAnswerButton.gameObject.SetActive(false);

        _canAcceptAnswers = true;
    }

    private void CorrectAnswer()
    {
        _answeringPlayerLabel.text = "";
        gameObject.SetActive(false);

        _answeringPlayer.UpdatePoints(_gameplayPlan.Price);

        Time.timeScale = 1.0f;
    }

    private void IncorrectAnswer()
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
                _timerLabel.text = (time - i).ToString(CultureInfo.InvariantCulture);

                yield return new WaitForSeconds(1);
            }

            gameObject.SetActive(false);
        }
    }
}