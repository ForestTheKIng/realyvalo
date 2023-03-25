using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] PhotonView playerPv;
    [SerializeField] TMP_Text text;

    void Start() {
        text.text = playerPv.Owner.NickName;    
    }
}
