using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.IO;
using ExitGames.Client.Photon.StructWrapping;
using Photon.Realtime;
using UnityEngine.Serialization;

public class PlantSpike : MonoBehaviourPunCallbacks
{
    public bool held;
    public bool trigged;
    public PhotonView pv;
    [System.NonSerialized]
    public bool defusing;
    public PlayerManager pm;
    public LocalPlayer lp;

    public TMP_Text _defuseText;

    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other){
        trigged = true;
    }
    void OnTriggerExit2D(Collider2D other){
        trigged = false;
    } 

    private void Start()
    {
        lp = GetComponent<LocalPlayer>();
        pm = lp.playerManager;
        pv = GetComponent<PhotonView>();
        _manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
    }
    


    void Update() {
        if (pv.IsMine)
        {
            if (defusing)
            {
                _defuseText.enabled = true;
            }
            else
            {
                _defuseText.enabled = false;
            }


            if (trigged == true)
            {
                if (Input.GetKeyDown("e"))
                {
                    held = true;
                    StartCoroutine(SpikePlant());
                }
            }
        }

    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetKey("f" ) && other.CompareTag("Spike") && pv.IsMine)
        {
            Debug.Log("defusing");
            defusing = true;
            StartCoroutine(pm.spikeDefuse());
        }
        else
        {
            Debug.Log("not defusing");
            defusing = false;
        }
    }
    public IEnumerator SpikePlant()
    {
        yield return new WaitForSeconds(4);
        if(held == true  && !pm.planted){
            PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Spike"), new Vector3(
                transform.position.x, transform.position.y,transform.position.z), Quaternion.identity, 0,
                new object[] {pv.ViewID});
            pm.planted = true;
            pm.UpdateTriggers(false);

        }
        else if (held == false)
        {
            Debug.Log("held is false");
        }
        else
        {
            Debug.Log("Spike is not null");
        }
    }
}
