using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using System.IO;

public class PlantSpike : MonoBehaviourPunCallbacks
{
    public TMP_Text defuText;
    public bool held;
    public bool trigged;
    private PhotonView pv; 
    [System.NonSerialized]
    public GameObject spike;

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
        pv = GetComponent<PhotonView>();
    }

    private void AssignVariables()
    {
        _spikeScript = spike.GetComponent<Spike>();
        _spikeScript.defuseText = defuText;
    }

    void Update() {
        if (trigged == true){
            
            if(Input.GetKeyDown("e")){
                held = true;
                StartCoroutine(SpikePlant());
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if(Input.GetKeyDown("e" ) && other.CompareTag("Spike")){
            _spikeScript.defusing = true;
            Debug.Log("defusing");
            StartCoroutine(_spikeScript.spikeDefuse());
        }
    }
    public IEnumerator SpikePlant()
    {
        yield return new WaitForSeconds(4);
        if(held == true  && spike == null){
            spike = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Spike"), new Vector3(
                transform.position.x, transform.position.y,transform.position.z), Quaternion.identity, 0,
                new object[] {pv.ViewID});
            AssignVariables();
        }
    }
}
