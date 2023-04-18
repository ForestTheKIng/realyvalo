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
    private PlayerManager _pm;
    public GameObject triggers;
    public GameObject spike;


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
                _pm = playerManager;

                // Stop searching for PlayerManagers
                break;
            }
        }

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
                Debug.Log("blue team ammount is: " + blueTeamPlayers + " red team ammount is: " + redTeamPlayers);

            }
            else
            {
                // If the player doesn't have a team assigned, assign them to a default team
                Debug.Log("Player " + player.NickName + " has no team assigned.");
            }
        }
    }

    public void UpdateScore(int teamNum)
    {
        pv.RPC("RPC_UpdateScore", RpcTarget.All, teamNum);
    }

    [PunRPC]
    public void RPC_UpdateScore(int teamNum)
    {
        if (teamNum == 0)
        {
            blueScore += 1;
        }
        else
        {
            redScore += 1;
        }
    }

    public void NewRound()
    {
        pv.RPC("RPC_NewRound", RpcTarget.All);
    }
    [PunRPC]
    public void RPC_NewRound(){
        spike = GameObject.Find("spike(Clone)");
        Debug.Log("new round");
        deadBlueTeamPlayers = 0;
        deadRedTeamPlayers = 0;
        currentTime = maxTime;
        startCurrentTime = startGameTime;
        gameStarted = false;
        pv.RPC("RPC_DestroySpike", RpcTarget.All);
        pv.RPC("RPC_UpdateTriggers", RpcTarget.All, true);
        if (_pm != null)
        {
            _pm.RunRPC();
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

    [PunRPC]
    void RPC_ResetScore()
    {
        blueScore = 0;
        redScore = 0;
    }

    [PunRPC]
    void RPC_DestroySpike()
    {
        Destroy(spike);
    }

    [PunRPC]
    void RPC_UpdateTriggers(bool active)
    {
        triggers.SetActive(active);
    }

    public void UpdateTriggers(bool active)
    {
        Debug.Log("gm update trig");
        pv.RPC("RPC_UpdateTriggers", RpcTarget.All, active);
    }


    void Update()
    {
        redScoreText.text = "Red: " + redScore;
        blueScoreText.text = "Blue: " + blueScore;
        

        if (redScore == 13){
            pv.RPC("RPC_ResetScore", RpcTarget.All);
        } else if (blueScore == 13){
            pv.RPC("RPC_ResetScore", RpcTarget.All);
        }

        if (deadBlueTeamPlayers == blueTeamPlayers) {
            UpdateScore(1);
            Debug.Log("no blue players");
            CallNewRoundRPC();
            
        } else if (deadRedTeamPlayers == redTeamPlayers) {
            UpdateScore(0);
            Debug.Log("No red players");
            NewRound(); 
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
