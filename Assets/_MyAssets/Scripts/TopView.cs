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
	string playerPlayFabID;
	int rankPlayFab;

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
				   LoginSuccess();
			   },
			   error => {
				   Debug.Log(error);
			   }
		   );
	}

	private void LoginSuccess()
	{
		GetUserData();
	}

	void SetUserData(string rank)
	{
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
		{
			Data = new Dictionary<string, string>() {
				{"Rank", rank}
			}
		},
		result => {
			Debug.Log("Successfully updated user data");
			PhotonNetwork.JoinLobby();
			_lobbyView.gameObject.SetActive(true);
			this.gameObject.SetActive(false);
		},
		error => {
			Debug.Log("Got error setting user data Ancestor to Arthur");
			Debug.Log(error.GenerateErrorReport());
		});
	}

	void GetUserData()
	{
		PlayFabClientAPI.GetUserData(new GetUserDataRequest()
		{
			PlayFabId = "",
			Keys = null
		}, result => {
			Debug.Log("Got user data:");
			if (result.Data == null || !result.Data.ContainsKey("Rank"))
			{
				Debug.Log("No Rank");
				int rank = 0;
				SetUserData(rank.ToString());
				SubmitScore();
			}
			else
			{
				Debug.Log("Rank: " + result.Data["Rank"].Value);
				PhotonNetwork.JoinLobby();
				_lobbyView.gameObject.SetActive(true);
				this.gameObject.SetActive(false);
			}
		}, (error) => {
			Debug.Log("Got error retrieving user data:");
			Debug.Log(error.GenerateErrorReport());
		});
	}

	//ランキングの情報を更新
	private void SubmitScore()
	{
		PlayFabClientAPI.UpdatePlayerStatistics(
			new UpdatePlayerStatisticsRequest
			{
				Statistics = new List<StatisticUpdate>()
				{
					new StatisticUpdate
					{
						StatisticName = "Janken",
						Value = 0
					}
				}
			},
			result => Debug.Log("score update"),
			error => Debug.Log(error)
		);
	}
}
