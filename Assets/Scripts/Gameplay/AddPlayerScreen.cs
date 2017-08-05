using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AddPlayerScreen : MonoBehaviour
{
    //[SerializeField] private InputField _inputField;
    //[SerializeField] private Button _inputButton;

    //private Player CreatedPlayer
    //{
    //    get
    //    {
    //        return new Player(_inputField.text, );
    //    }
    //}

    //public void Show(Action<Player> callback)
    //{
    //    gameObject.SetActive(true);


    //}

    //private IEnumerator WaitForInput()
    //{
        //while (true)
        //{
        //    foreach (KeyCode vKey in Enum.GetValues(typeof(KeyCode)))
        //    {
        //        if (Input.GetKey(vKey))
        //        {
        //            var player = new Player(_playerName.text, vKey);
        //            _activePlayers.Add(player);

        //            var instantiatedPlayerEntity = Instantiate(_playerEntityTemplate);
        //            instantiatedPlayerEntity.Init(player, OnPlayerDeletedHandler);

        //            instantiatedPlayerEntity.transform.SetParent(_playersList, false);
        //            Debug.Log("Player added. Name: " + _playerName.text + " key: " + vKey);

        //            _playerName.text = "";

        //            yield break;
        //        }
        //    }

        //    yield return null;
        //}
    //}
}