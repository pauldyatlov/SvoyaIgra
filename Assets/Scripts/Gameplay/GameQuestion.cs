using System;
using UnityEngine;
using UnityEngine.UI;

public class GameQuestion : MonoBehaviour
{
    [SerializeField] private Button _questionButton;
    [SerializeField] private Text _priceLabel;

    public void Init(string price, RectTransform parent, Action onQuestionPressed)
    {
        transform.SetParent(parent, false);
        _priceLabel.text = price;

        _questionButton.onClick.AddListener(() => onQuestionPressed());
    }
}