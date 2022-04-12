using UnityEngine;
using System.Collections;
using Photon.Pun;
using Photon.Realtime;
using System;

public enum PhotonState
{ 
    None = 0,
    Offline,
    Online,
    InLobby,
    InRoom,
}

public class SimplePUNManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private LobbyView _lobbyView = null;
    [SerializeField] private JankenView _jankenView = null;

    private bool _dontDestroyOnLoad;
    private PhotonState _connectionState = PhotonState.None;
    private Player _rivalPlayer = null;

    private static SimplePUNManager _instance;
    public static SimplePUNManager Instance
    {
        get
        {
            if (!_instance)
            {
                Type t = typeof(SimplePUNManager);
                _instance = (SimplePUNManager)FindObjectOfType(t);
                if (!_instance)
                {
                    Debug.LogError(t + " is nothing.");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        if (_dontDestroyOnLoad)
        {
            DontDestroyOnLoad(this.gameObject);
        }
    }

	private void Start()
	{
        Connection();
	}

	private void OnApplicationQuit()
	{
        PhotonNetwork.Disconnect();
	}

	void OnGUI()
    {
        //ÉçÉOÉCÉìÇÃèÛë‘ÇâÊñ è„Ç…èoóÕ
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        GUILayout.Label(_connectionState.ToString());
    }

    public void Connection()
	{
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom("Janken");
	}

    public void JoinRoom()
	{
        PhotonNetwork.JoinRoom("Janken");
	}
       
    public override void OnConnectedToMaster()
    {
        _connectionState = PhotonState.Online;
        // PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
	{
        _connectionState = PhotonState.InLobby;
        _lobbyView.gameObject.SetActive(true);
    }

    public override void OnJoinedRoom()
    {
        _connectionState = PhotonState.InRoom;
        _lobbyView.gameObject.SetActive(false);
        if(PhotonNetwork.PlayerListOthers.Length != 0)
		{
            Debug.Log("OtherPlayer Already Exists.");
            _rivalPlayer = PhotonNetwork.PlayerListOthers[0];
            _jankenView.SetRivalName(_rivalPlayer.NickName);
            _jankenView.gameObject.SetActive(true);
        }
    }

	public override void OnCreatedRoom()
	{
        _connectionState = PhotonState.InRoom;
        _lobbyView.gameObject.SetActive(false);
    }

	public override void OnLeftRoom()
	{
        _connectionState = PhotonState.InLobby;
        _lobbyView.gameObject.SetActive(true);
        _jankenView.gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
	{
        _rivalPlayer = newPlayer;
        _jankenView.gameObject.SetActive(true);
	}

	public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _rivalPlayer = null;
        PhotonNetwork.LeaveRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
	{
        Debug.LogError($"{returnCode}: {message}");
	}

	public override void OnJoinRoomFailed(short returnCode, string message)
	{
        Debug.LogError($"{returnCode}: {message}");
    }
}
