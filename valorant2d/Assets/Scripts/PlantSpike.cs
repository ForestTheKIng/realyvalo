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
    private PhotonView pv; 
    [System.NonSerialized]
    public GameObject spike;
    public bool defusing;
    public PlayerManager pm;
    public LocalPlayer lp;

    public TMP_Text _defuseText;

    [FormerlySerializedAs("_spikeScript")] public Spike spikeScript;
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
        }

        if (trigged == true){
            if(Input.GetKeyDown("e")){
                held = true;
                StartCoroutine(SpikePlant());
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
            spike = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Spike"), new Vector3(
                transform.position.x, transform.position.y,transform.position.z), Quaternion.identity, 0,
                new object[] {pv.ViewID});
            pm.planted = true;
            pm.UpdateTriggers(false);

        }
    }
}
