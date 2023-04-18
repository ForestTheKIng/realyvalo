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
        SpikePlanted = true;
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
        manager.spikeScript = GetComponent<Spike>();
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