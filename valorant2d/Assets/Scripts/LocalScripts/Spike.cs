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

    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
    }

    

    public void Update()
    {
        _spikeTimer -= 1 * Time.deltaTime;

        if (_spikeTimer <= 0)
        {
            explode();
        }
    }
    

    public void explode()
    {
        manager.redScore += 1;
        manager.NewRound(); 
        PhotonNetwork.Destroy(this.gameObject);
    }
}