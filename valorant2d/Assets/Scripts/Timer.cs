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
    public int deadBlueTeamPlayers;
    public int deadRedTeamPlayers;

    // Update is called once per frame
    private void Start() {
        Player[] players = PhotonNetwork.PlayerList;

        // Iterate through each player and check what team they are on
        foreach (Player player in players)
        {
            // Get the player's team number from their custom properties
            if (player.CustomProperties.TryGetValue("team", out object teamObj))
            {
                int team = (int)teamObj;
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


    void Update()
    {
        if (deadBlueTeamPlayers == blueTeamPlayers) {
            Debug.Log("red won");
        } else if (deadRedTeamPlayers == redTeamPlayers) {
            Debug.Log("blue won");
        }

        if (gameStarted == true){
            currentTime -= 1 * Time.deltaTime;
            int currentTimeInt = (int) Math.Round(currentTime);
            timerText.text = currentTimeInt.ToString();
            if (currentTime <= 0){
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
        } else {
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
