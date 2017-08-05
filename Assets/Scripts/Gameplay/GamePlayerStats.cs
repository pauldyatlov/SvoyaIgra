using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayerStats : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Text _nameLabel;
    [SerializeField] private Text _pointsLabel;

    [SerializeField] private CanvasGroup _canvasGroup;

    public Action<Player> OnPlayerSelectedAction;
    private Player _player;

    public void Init(Player player)
    {
        _player = player;

        _nameLabel.text = player.Name;
        _pointsLabel.text = player.Points.ToString();

        player.OnPointsUpdateAction += arg =>
        {
            _nameLabel.text = arg.Name.ToString();

            if (_pointsLabel != null)
                _pointsLabel.text = arg.Points.ToString();
        };
    }

    public void SetCanvasGroup(bool value)
    {
        _canvasGroup.alpha = value ? 1 : 0.3f;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnPlayerSelectedAction?.Invoke(_player);
    }
}