using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;
using TMPro;
using Photon.Realtime;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;

    private const string TEAM_PROPERTY_KEY = "team";

    private List<RoomInfo> availableRooms = new List<RoomInfo>();

    [SerializeField] TMP_InputField locateRoomInputField;
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    [SerializeField] TMP_Text joinRoomErrorText;

    public TMP_Text teamText;

    public PhotonView playerPrefab;

    public override void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    public override void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    
    void Awake(){
        Instance = this;
    }

    void Start(){
        Debug.Log("Connecting to server...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster(){
        Debug.Log("Joining lobby...");
        PhotonNetwork.JoinLobby();
        Debug.Log("joinlobby called");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby(){
        MenuManager.Instance.OpenMenu("title");
        Debug.Log("Joined Lobby");
    }

    public void JoinTheLobby(){
        // PhotonNetwork.JoinLobby();
        Debug.Log("Joining lobby again...");
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom(){
        if(string.IsNullOrEmpty(roomNameInputField.text)){
            return;
        }
        PhotonNetwork.CreateRoom(roomNameInputField.text);
        Debug.Log("Created room");
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnJoinedRoom(){

        Debug.Log("Connected to room");
        MenuManager.Instance.OpenMenu("room"); 
        roomText.text = PhotonNetwork.CurrentRoom.Name;
        
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent){
            Destroy(child.gameObject);
        }
        
        for (int i = 0; i < players.Count(); i++)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        // Assign player to a team when they join the room
        int playerTeam = GetSmallestTeam();
        Debug.Log("Assigning to a team...");
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            {TEAM_PROPERTY_KEY, playerTeam}
        });
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);


    }
    
    public override void  OnMasterClientSwitched(Photon.Realtime.Player newMasterClient){
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        errorText.text = "Room Creation Failed " + message;
        MenuManager.Instance.OpenMenu("error");
    }

    public void LeaveRoom(){
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    
    
    public void JoinRoom()
    {
        string roomName = locateRoomInputField.text;
        
        foreach (RoomInfo room in availableRooms)
        {
            Debug.Log("This room is " + room.Name);
            if (room.Name == roomName)
            {
                PhotonNetwork.JoinRoom(roomName);
                return;
            }
        }
        Debug.Log("Room join failed" + roomName);
    }

    public override void OnLeftRoom(){
        MenuManager.Instance.OpenMenu("title");
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        availableRooms = roomList;
    }

    public void Update() {
        Photon.Realtime.Player player = PhotonNetwork.LocalPlayer; // or replace with the desired player object

        if (player.CustomProperties.ContainsKey(TEAM_PROPERTY_KEY))
        {
            object teamObj = player.CustomProperties[TEAM_PROPERTY_KEY];
            int team = (int)teamObj;
            teamText.text = "Player " + player.NickName + " is on team " + team;
        }
        else
        {
            teamText.text = "Player " + player.NickName + " does not have a team assigned.";
        }
        
    }


    private int GetSmallestTeam()
    {
        int blueTeamCount = 0;
        int redTeamCount = 0;
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            object teamObj;
            if (player.CustomProperties.TryGetValue(TEAM_PROPERTY_KEY, out teamObj))
            {
                int team = (int)teamObj;
                if (team == 0) blueTeamCount++;
                else redTeamCount++;
            }
        }
        Debug.Log("Team Asigned!!!");
        return (blueTeamCount <= redTeamCount) ? 0 : 1;
    }
    


    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer){
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    public void StartGame(){
        PhotonNetwork.LoadLevel(1);
    }
}
