using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using Photon.Pun;
using Photon.Realtime;


public class Timer : MonoBehaviourPunCallbacks
{
    public TMP_Text timerText;
    const float maxTime = 180f;
    private float currentTime = maxTime;

    const float startGameTime = 20f;
    private float startCurrentTime = startGameTime;

    public bool gameStarted = false;
    public GameObject barriers;

    private int blueTeamPlayers;
    private int redTeamPlayers;
    public int deadBlueTeamPlayers = 0;
    public int deadRedTeamPlayers = 0;
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;
    public int blueScore = 0;
    public int redScore = 0;

    private int team;

    private void Awake() {
        blueScoreText = GameObject.Find("BlueScoreText").GetComponent<TMP_Text>();
        redScoreText = GameObject.Find("RedScoreText").GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    private void Start() {
        Player[] players = PhotonNetwork.PlayerList;

        // Iterate through each player and check what team they are on
        foreach (Player player in players)
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


    public void NewRound(){
        deadBlueTeamPlayers = 0;
        deadRedTeamPlayers = 0;
        // Call the "MyFunction" function on all instances of "MyScript
        gameStarted = true;
        photonView.RPC("RPC_CreateController", RpcTarget.All, false);
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
            redScore += 1;
            NewRound();
            
        } else if (deadRedTeamPlayers == redTeamPlayers) {
            blueScore += 1;
            NewRound();
        }

        if (gameStarted == true){
            currentTime -= 1 * Time.deltaTime;
            int currentTimeInt = (int) Math.Round(currentTime);
            timerText.text = currentTimeInt.ToString();
            if (currentTime <= 0){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
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
