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
    public float _spikeTimer = 46;
    public bool defusing;
    public TMP_Text defuseText;
    public bool SpikePlanted;

    private GameManager manager;
    // Start is called before the first frame update
    void Start()
    {
        SpikePlanted = true;
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
        manager.spike = GetComponent<Spike>();
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
        
    }
    

    public void explode()
    {
        manager.redScore += 1;
        manager.NewRound();
        PhotonNetwork.Destroy(this.gameObject);
    }
}