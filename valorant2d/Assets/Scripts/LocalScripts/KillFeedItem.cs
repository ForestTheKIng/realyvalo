using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class KillFeedItem : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text text;

    public void SetUp(string killer, string killed){
        text.text = killer + " " + "brutally murdered" + " " + killed;
    }

}

