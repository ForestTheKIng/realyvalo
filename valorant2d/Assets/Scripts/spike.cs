using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Spike : MonoBehaviourPunCallbacks
{
    private float _spikeTimer = 1000;
    public bool defusing;
    public TMP_Text defuseText;

    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
    }



    public IEnumerator spikeDefuse()
    {   

        yield return new WaitForSeconds(4);
        if(defusing == true)
        {
            manager.blueScore += 1;
            manager.NewRound();
        }
    }

    public void Update()
    {
        _spikeTimer -= 1 * Time.deltaTime;

        if (_spikeTimer <= 0)
        {
            explode();
        }

        if (defusing)
        {
            defuseText.text = "defusing...";
        }
        else if (defusing == false)
        {
            defuseText.text = "";
        }
    }
    

    public void explode()
    {
        manager.redScore += 1;
        manager.NewRound();
        PhotonNetwork.Destroy(this.gameObject);
    }
}