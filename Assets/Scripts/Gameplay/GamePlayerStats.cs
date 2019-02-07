using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GamePlayerStats : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Color _defaultLabelColor;

    [SerializeField] private Text _nameLabel;
    [SerializeField] private Text _pointsLabel;

    [SerializeField] private CanvasGroup _canvasGroup;

    private event Action<Player> OnPlayerSelected;
    private Player _player;

    public void Init(Player player, Action<Player> onPlayerSelected)
    {
        _player = player;
        OnPlayerSelected = onPlayerSelected;

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

    public void SetConnectedStatus(bool value)
    {
        SetCanvasGroup(value);

        _nameLabel.color = value ? _defaultLabelColor : Color.red;
        _pointsLabel.color = value ? _defaultLabelColor : Color.red;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (eventData.button)
        {
            case PointerEventData.InputButton.Left:
            case PointerEventData.InputButton.Right:
            case PointerEventData.InputButton.Middle:
            {
                OnPlayerSelected?.Invoke(_player);

                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}