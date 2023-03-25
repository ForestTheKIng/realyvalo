using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    SpawnPoint[] spawnpoints;
     
    void Awake() {
        Instance = this;  
        spawnpoints = GetComponentsInChildren<SpawnPoint>();  
    }

    public Transform GetSpawnPoint(int team){
        if (team == 0){
            return spawnpoints[0].transform;
        } else if (team == 1){
            return spawnpoints[1].transform;
        } else {
            return spawnpoints[1].transform;
        }        
    }
}
