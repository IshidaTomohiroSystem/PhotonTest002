using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using Photon.Pun;
using Photon.Realtime;

public class TopView : MonoBehaviour
{
	[SerializeField] private Button _loginButton = null;
	[SerializeField] private LobbyView _lobbyView = null;
	[SerializeField] private InputField _playerId = null;

	private void Start()
	{
		_loginButton.onClick.AddListener(Login);
	}

	private void Login()
	{
		if(_playerId.text.Length == 0)
        {
			Debug.Log("please input id");
			return;
        }

		PlayFabClientAPI.LoginWithCustomID(
			   new LoginWithCustomIDRequest
			   {
				   CustomId = _playerId.text,
				   CreateAccount = true
			   },
			   result => {
				   Debug.Log("Login success");
				   PhotonNetwork.JoinLobby();
				   _lobbyView.gameObject.SetActive(true);
				   this.gameObject.SetActive(false);
			   },
			   error => {
				   Debug.Log(error);
			   }
		   );
	}
}
