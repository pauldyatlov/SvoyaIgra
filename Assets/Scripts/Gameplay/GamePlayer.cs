using UnityEngine;
using UnityEngine.UI;

public class GamePlayer : MonoBehaviour
{
    [SerializeField] private Text _playerLabel;

    public void Init(Player player)
    {
        _playerLabel.text = "NAME: " + player.Name + " CODE: " + player.Code;
    }
}