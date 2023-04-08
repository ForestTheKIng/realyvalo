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
    public PhotonView pv;
    [System.NonSerialized]
    
    private GameManager _manager;
    
    public Spike _spikeScript;
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

    public void AssignVariables()
    {
        Debug.Log("Assigning Vars");
        _spikeScript = _manager.spike.GetComponent<Spike>();
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
        if (_manager.spike != null && Input.GetKey("f") && other.CompareTag("Spike") && pv.IsMine)
        {   
            Debug.Log(_spikeScript);
            _spikeScript.defusing = true;
            Debug.Log("defusing");
            StartCoroutine(_spikeScript.spikeDefuse());
        } else if (Input.GetKeyUp("f"))
        {
            Debug.Log("key up");
        } else if (other.CompareTag("Spike") == false)
        {
            Debug.Log("tag wrong");
        } else if (pv.IsMine == false)
        {
            Debug.Log("pv is wrong");
        }
        else if (_manager.spike == null)
        {
            Debug.Log("Spike is null");
        }
        else
        {
            Debug.Log("Unknown Error");
        }
    }
    public IEnumerator SpikePlant()
    {
        yield return new WaitForSeconds(4);
        if(held == true  && _manager.spike == null){
            Debug.Log("planting spike");
            _manager.UpdateTriggers(false);
            _manager.spike = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Spike"), new Vector3(
                transform.position.x, transform.position.y,transform.position.z), Quaternion.identity, 0,
                new object[] {pv.ViewID});
            _manager.UpdateSpike();
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
