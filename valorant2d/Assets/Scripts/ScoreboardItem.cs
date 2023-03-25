using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;
using Photon.Pun;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreboardItem : MonoBehaviourPunCallbacks
{
    public TMP_Text usernameText;
    public TMP_Text killsText;
    public TMP_Text deathsText;
    public TMP_Text blueScoreText;
    public TMP_Text redScoreText;
    private const string TEAM_PROPERTY_KEY = "team";

    Player player;

    public void Initialize(Player player){
        usernameText.text = player.NickName;
        this.player = player;
        blueScoreText = GameObject.Find("BlueScoreText").GetComponent<TMP_Text>();
        redScoreText = GameObject.Find("RedScoreText").GetComponent<TMP_Text>();
        UpdateStats();
    }

    void UpdateStats(){
        if (player.CustomProperties.TryGetValue("kills", out object kills)){
            killsText.text = kills.ToString();
            if (player.CustomProperties.ContainsKey(TEAM_PROPERTY_KEY))
            {
                object teamObj = player.CustomProperties[TEAM_PROPERTY_KEY];
                int team = (int)teamObj;
                if (team == 0) {
                    blueScoreText.text = "BlueTeamScore: " + kills.ToString();
                } else if (team == 1) {
                    redScoreText.text = "RedTeamScore: " + kills.ToString();
                } else {
                    Debug.LogError("Erorr: No team assigned");
                }
            }
        }
        if (player.CustomProperties.TryGetValue("deaths", out object deaths)){
            deathsText.text = deaths.ToString();
        }

        
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps){
        if (targetPlayer == player){
            if (changedProps.ContainsKey("kills") || changedProps.ContainsKey("deaths")){
                UpdateStats();
            }
        }
    }
}
