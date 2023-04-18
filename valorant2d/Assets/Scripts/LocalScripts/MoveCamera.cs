using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveCamera : MonoBehaviourPunCallbacks
{
    public PhotonView pv;
    public GameObject player;
    public GameObject cam;
    public Vector3 CamPos;


    private void Awake() {
        cam = gameObject;   
    }
    void Update()
    {
        if(pv.IsMine){
            CamPos = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
            cam.transform.position = CamPos;
        }else{
            gameObject.SetActive(false);
        }

    }
}
    