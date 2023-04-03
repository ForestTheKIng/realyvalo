using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

public class Spike : MonoBehaviour
{
    private int SpikeTimer;
    [FormerlySerializedAs("held")] [SerializeField] public bool defusing;
    public TMP_Text defuseText;

    private Timer manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<Timer>();
        StartCoroutine(DetonateTimer());
    }

    private void OnTriggerStay(Collider other)
    {
        if(Input.GetKeyDown("e")){
            defusing = true;
            StartCoroutine(spikeDefuse());
        }
    }
    
    public IEnumerator spikeDefuse()
    {

        yield return new WaitForSeconds(4);
        if(defusing == true)
        {
            manager.blueScore += 1;
            manager.NewRound();
        }
    }

    public void Update()
    {
        if (SpikeTimer >= 46)
        {
            explode();
        }

        if (defusing)
        {
            defuseText.text = "defusing...";
        }
        else if (defusing == false)
        {
            defuseText.text = "";
        }
    }

    public IEnumerator DetonateTimer(){
        yield return new WaitForSeconds(1);
        SpikeTimer += 1;
        Debug.Log(SpikeTimer);
    }

    public void explode()
    {
        manager.redScore += 1;
        manager.NewRound();
    }
}