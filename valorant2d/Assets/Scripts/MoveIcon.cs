using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class MoveIcon : MonoBehaviour
{
    private Transform _player;
    private void Start()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject player in players)
        {
            PhotonView pv = player.GetComponent<PhotonView>();
            if (pv.IsMine)
            {
                _player = player.transform;
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = _player.position;
    }
}
