using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;

public enum Commands
{ 
	None = 0,
	Gu,
	Choki,
	Pa
}

public enum BattleResult
{ 
	Draw = 0,
	Win,
	Lose,
}

public class JankenView : MonoBehaviour
{
	public const byte SetCommandEventCode = 111;

	[SerializeField] Button _commandButton1 = null;
	[SerializeField] Button _commandButton2 = null;
	[SerializeField] Button _commandButton3 = null;

	[SerializeField] DispCommandView _selfDisplay = null;
	[SerializeField] DispCommandView _rivalDisplay = null;

	[SerializeField] TextMeshProUGUI _rivalName = null;
	[SerializeField] TextMeshProUGUI _messageText = null;

	private Commands _selfCommand;
	private Commands _rivalCommand;
	private BattleResult _result;

	private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();

	private void Awake()
	{
		_commandButton1.onClick.AddListener(() => SetCommand(Commands.Gu));
		_commandButton2.onClick.AddListener(() => SetCommand(Commands.Choki));
		_commandButton3.onClick.AddListener(() => SetCommand(Commands.Pa));
	}
	private void OnEnable()
	{
		InitGame();
		PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
	}

	private void OnDisable()
	{
		InitGame();
		PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
	}

	public void SetRivalName(string name)
	{
		_rivalName.text = name;
	}

	public void ResetRivalName()
	{
		_rivalName.text = "";
	}

	private void InitGame()
	{
		_cancelTokenSource.Cancel();
		_selfCommand = Commands.None;
		_rivalCommand = Commands.None;
		_selfDisplay.Reset();
		_rivalDisplay.Reset();
		_commandButton1.interactable = true;
		_commandButton2.interactable = true;
		_commandButton3.interactable = true;
		_messageText.text = "Select command...";
	}

	private void SetCommand(Commands command)
	{
		_selfCommand = command;
		_selfDisplay.SetCommandSprite(command);
		_selfDisplay.ShowDown();
		_commandButton1.interactable = false;
		_commandButton2.interactable = false;
		_commandButton3.interactable = false;

		RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
		PhotonNetwork.RaiseEvent(SetCommandEventCode, (int)_selfCommand, raiseEventOptions, SendOptions.SendReliable);
		_cancelTokenSource = new CancellationTokenSource();
		WaitingCommand(_cancelTokenSource.Token).Forget();
	}

	public void OnEvent(EventData eventData)
	{
		var eventCode = eventData.Code;
		if (eventCode == SetCommandEventCode)
		{
			object data = eventData.CustomData;
			Debug.Log($"Receive Event {(data != null ? data : "nodata")}");
			_rivalCommand = (Commands)(int)data;
		}
	}

	private async UniTask WaitingCommand(CancellationToken cancelToken)
	{
		await UniTask.WaitUntil(() => _selfCommand != Commands.None && _rivalCommand != Commands.None, cancellationToken: cancelToken);

		_rivalDisplay.SetCommandSprite(_rivalCommand);
		_rivalDisplay.ShowDown();

		// Gu > Choki
		// Choki > Pa
		// Pa > Gu
		if(_selfCommand == _rivalCommand)
		{
			// Ç†Ç¢Ç±
			_result = BattleResult.Draw;
		}
		else if (_selfCommand == Commands.Gu && _rivalCommand == Commands.Choki
		|| _selfCommand == Commands.Choki && _rivalCommand == Commands.Pa
		|| _selfCommand == Commands.Pa && _rivalCommand == Commands.Gu)
		{
			// èüÇø
			_result = BattleResult.Win;
		}
		else
		{
			// ïâÇØ
			_result = BattleResult.Lose;
		}

		switch(_result)
		{
			case BattleResult.Draw:
				_messageText.text = "Draw";
				break;
			case BattleResult.Win:
				_messageText.text = "You Win!";
				GetUserData();
				break;
			case BattleResult.Lose:
				_messageText.text = "You Lose...";
				break;
		}

		await UniTask.Delay(3000, cancellationToken: cancelToken);

		InitGame();
	}

	private void SubmitScore(int score)
	{
		PlayFabClientAPI.UpdatePlayerStatistics(
			new UpdatePlayerStatisticsRequest
			{
				Statistics = new List<StatisticUpdate>()
				{
					new StatisticUpdate
					{
						StatisticName = "Janken",
						Value = score
					}
				}
			},
			result => Debug.Log("score update"),
			error => Debug.Log(error)
		);
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
				SubmitScore(0);
			}
			else
			{
				Debug.Log("Rank: " + result.Data["Rank"].Value);
				int updateScore = int.Parse(result.Data["Rank"].Value) + 1;
				SetUserData(updateScore);
			}
		}, (error) => {
			Debug.Log("Got error retrieving user data:");
			Debug.Log(error.GenerateErrorReport());
		});
	}

	void SetUserData(int score)
	{
		PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
		{
			Data = new Dictionary<string, string>() {
			{"Rank", score.ToString()}
		}
		},
		result => {
			Debug.Log("Successfully updated user data");
			SubmitScore(score);
		},
		error => {
			Debug.Log("Got error setting user data Ancestor to Arthur");
			Debug.Log(error.GenerateErrorReport());
		});
	}
}
