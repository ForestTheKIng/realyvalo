using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Serialization;


public class GameManager : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    const float maxTime = 300f;
    private float currentTime = maxTime;

    const float startGameTime = 20f;
    private float startCurrentTime = startGameTime;

    public bool gameStarted = false;
    public GameObject barriers;

    public int blueTeamPlayers;
    public int redTeamPlayers;
    public int deadBlueTeamPlayers = 0;
    public int deadRedTeamPlayers = 0;
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;
    public int blueScore = 0;
    public int redScore = 0;
    private PhotonView pv;
    private PlayerManager manager;
    [FormerlySerializedAs("spike")] public Spike spikeScript;
    public GameObject spike;
    public GameObject triggers;
    
    private int team;

    private void Awake() {
        pv = GetComponent<PhotonView>();
        blueScoreText = GameObject.Find("BlueScoreText").GetComponent<TMP_Text>();
        redScoreText = GameObject.Find("RedScoreText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    private void Start() {
        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        // Iterate through each player and check what team they are on
        foreach (Photon.Realtime.Player player in players)
        {
            // Get the player's team number from their custom properties
            if (player.CustomProperties.TryGetValue("team", out object teamObj))
            {
                team = (int)teamObj;
                if (team == 0) {
                    blueTeamPlayers += 1;
                } else if (team == 1) {
                    redTeamPlayers += 1;
                }
            }
        }
    }

    public void UpdateSpike()
    {
        pv.RPC("RPC_UpdateSpike",RpcTarget.All);
    }
    
    [PunRPC]
    public void RPC_UpdateSpike()
    {
        if (spike == null)
        {
            spike = GameObject.Find("spike(Clone)");
        }
        
        PlayerManager[] playerManagers = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager playerManager in playerManagers)
        {
            if (playerManager.pv.IsMine)
            {
                // Set myPlayerManager to the PlayerManager owned by the local client
                manager = playerManager;

                // Stop searching for PlayerManagers
                break;
            }
        }

        Debug.Log("running var func");
        manager.plantSpikeScript.AssignVariables();
    }

    public void CallNewRoundRPC()
    {
        pv.RPC("RPC_NewRound", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_NewRound()
    {
        Debug.Log("new round");
        deadBlueTeamPlayers = 0;
        deadRedTeamPlayers = 0;
        // Find all instances of PlayerManager in the scene
        PlayerManager[] playerManagers = FindObjectsOfType<PlayerManager>();

        // Loop through each PlayerManager to find the one owned by the local client
        foreach (PlayerManager playerManager in playerManagers)
        {
            if (playerManager.pv.IsMine)
            {
                // Set myPlayerManager to the PlayerManager owned by the local client
                manager = playerManager;

                // Stop searching for PlayerManagers
                break;
            }
        }

        gameStarted = true;
        if (manager != null)
        {
            manager.RunRPC();
        }
        
    }

    public void UpdateScore(int rpcTeam)
    {
        pv.RPC("RPC_UpdateScore", RpcTarget.All, rpcTeam);
    }
    
    public void UpdateTriggers(bool active)
    {
        pv.RPC("RPC_UpdateTriggers", RpcTarget.All, active);
    }

    [PunRPC]
    public void RPC_UpdateTriggers(bool active)
    {
        triggers = GameObject.Find("triggers");
        triggers.SetActive(active);
    }

    [PunRPC]
    public void RPC_UpdateScore(int rpcTeam)
    {
        if (team == 0)
        {
            blueScore += 1;
        }
        else if (team == 1)
        {
            redScore += 1;
        }
    }


    void Update()
    {
        redScoreText.text = "Red: " + redScore;
        blueScoreText.text = "Blue: " + blueScore;

        if (redScore == 13){
            Debug.Log("red won");
        } else if (blueScore == 13){
            Debug.Log("blue won");
        }

        if (deadBlueTeamPlayers == blueTeamPlayers) {
            UpdateScore(1);
            Debug.Log("no blue players");
            CallNewRoundRPC();
            
        }
        
        if (deadRedTeamPlayers == redTeamPlayers) {
            blueScore += 1;
            Debug.Log("No red players");
            CallNewRoundRPC();
        }

        if (gameStarted){
            currentTime -= 1 * Time.deltaTime;
            int currentTimeInt = (int) Math.Round(currentTime);
            timerText.text = currentTimeInt.ToString();
            if (currentTime <= 0){
                // Add new round
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } else if (spikeScript != null && spikeScript.SpikePlanted)
        {
            currentTime = spikeScript._spikeTimer;
            var currentTimeInt = (int) Math.Round(currentTime);
            timerText.text = currentTimeInt.ToString();
        } else {
            barriers.SetActive(true);
            startCurrentTime -= 1 * Time.deltaTime;
            int startTimeInt = (int) Math.Round(startCurrentTime);
            timerText.text = startTimeInt.ToString();
            if (startCurrentTime <= 0){
                gameStarted = true;
                barriers.SetActive(false);
            }
        }
    }
}
