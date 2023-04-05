using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;

public class PlantSpike : MonoBehaviourPunCallbacks
{
    private LocalPlayer playerScript;
    public TMP_Text defuText;
    public bool held;
    public bool trigged;
    private PhotonView pv;
    public PhotonView playerPv;
    [System.NonSerialized]
    public GameObject spike;
    public GameObject triggers;

    private GameManager _manager;
    
    private Spike _spikeScript;
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other){
        trigged = true;
    }
    void OnTriggerExit2D(Collider2D other){
        trigged = false;
    }

    private void Start()
    {
        playerScript = GetComponent<LocalPlayer>();
        pv = GetComponent<PhotonView>();
        _manager = GameObject.Find("ScoreboardCanvas").GetComponent<GameManager>();
    }

    private void AssignVariables()
    {
        _spikeScript = spike.GetComponent<Spike>();
        _spikeScript.defuseText = defuText;
    }

    void Update() {
        if (trigged == true){
            
            if(Input.GetKeyDown("e") && pv.IsMine && playerScript.team == 1){
                held = true;
                StartCoroutine(SpikePlant());
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (spike != null)
        {
            playerPv = other.gameObject.GetComponent<PhotonView>();
            if (Input.GetKeyDown("f") && other.CompareTag("Spike") && playerPv.IsMine)
            {
                _spikeScript.defusing = true;
                Debug.Log("defusing");
                StartCoroutine(_spikeScript.spikeDefuse());
            }
        }
    }
    public IEnumerator SpikePlant()
    {
        yield return new WaitForSeconds(4);
        if(held == true  && spike == null){
            Debug.Log("planting spike");
            triggers = GameObject.Find("triggers");
            _manager.UpdateTriggers(false);
            spike = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Spike"), new Vector3(
                transform.position.x, transform.position.y,transform.position.z), Quaternion.identity, 0,
                new object[] {pv.ViewID});
            AssignVariables();
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
