using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerEntity : MonoBehaviour
{
    [SerializeField] private Text _playerLabel;

    public void Init(Player player)
    {
        _playerLabel.text = "NAME: " + player.Name + " CODE: " + player.Code;
    }
}
