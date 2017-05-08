using UnityEngine;
using UnityEngine.UI;

public class PlayerStatsEntity : MonoBehaviour
{
    [SerializeField] private Text _nameLabel;
    [SerializeField] private Text _keycodeLabel;
    [SerializeField] private Text _pointsLabel;

    public void Init(Player player)
    {
        player.OnPointsUpdateAction += arg =>
        {
            _nameLabel.text = arg.Name.ToString();
            _pointsLabel.text = arg.Points.ToString();
            _keycodeLabel.text = arg.Code.ToString();
        };
    } 
}
