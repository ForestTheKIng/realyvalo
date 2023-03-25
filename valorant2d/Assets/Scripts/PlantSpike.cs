using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlantSpike : MonoBehaviourPunCallbacks
{
    public bool held;
    public bool trigged;

    public GameObject spike;
    // Update is called once per frame
    void OnTriggerEnter2D(Collider2D other){
        trigged = true;
    }
    void OnTriggerExit2D(Collider2D other){
        trigged = false;
    }
 
    void Update() {
        if (trigged == true){
            
            if(Input.GetKeyDown("e")){
                held = true;
                StartCoroutine(spikePlant());
            }
        }
    }
    public IEnumerator spikePlant()
    {

        yield return new WaitForSeconds(4);
        if(held == true){
            PhotonNetwork.Instantiate(spike.name, new Vector3(transform.position.x, transform.position.y,transform.position.z), Quaternion.identity);
        }
    }
}
