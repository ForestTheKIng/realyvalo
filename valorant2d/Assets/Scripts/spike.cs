using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spike : MonoBehaviour
{
    private int SpikeTimer;
    // Start is called before the first frame update
    void Start(){
        StartCoroutine(DetonateTimer());
    }

    public void Update(){
        if(SpikeTimer >= 46){
            explode();
        }
    }

    public IEnumerator DetonateTimer(){
        yield return new WaitForSeconds(1);
        SpikeTimer += 1;
        Debug.Log(SpikeTimer);
    }

    public void explode(){
        // need win con code first
        Debug.Log("EXPLODED PEW PEW");
    }
}