using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyView : MonoBehaviour
{
	[SerializeField] private Button _CreateButton = null;
	[SerializeField] private Button _joinButton = null;

	private void Start()
	{
		_joinButton.onClick.AddListener(Join);
		_CreateButton.onClick.AddListener(Create);
	}

	private void Join()
	{
		SimplePUNManager.Instance.JoinRoom();
	}

	private void Create()
	{
		SimplePUNManager.Instance.CreateRoom();
	}

}
