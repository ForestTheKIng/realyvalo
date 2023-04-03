using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using ExitGames.Client.Photon.StructWrapping;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    
    public PhotonView pv;

    public GameObject controller;

    int kills;
    int deaths;
    
    public GameObject killFeedItem;

    public int roundNumber;

    [SerializeField] GameObject killFeedItemPrefab;
    [SerializeField] Transform killFeedContent;
    public Timer manager;

    private const string TEAM_PROPERTY_KEY = "team";
    private bool spectate = false;
    private GameObject specCam;
    private GameObject mCam;
    private PlantSpike plantSpikeScript;
    private Spike spikeScript;

    void Awake()
    {
        pv = GetComponent<PhotonView>();
        Settings.Instance.SceneChanged();
        

        // killFeedItemPrefab = Path.Combine("Prefabs", "KillFeedItem");
    }
    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.Find("ScoreboardCanvas").GetComponent<Timer>();

        if (pv.IsMine){
            CreateController();
        }
    }



    void SpectateCam() {
        spikeScript = plantSpikeScript.spike.GetComponent<Spike>();
        mCam = controller.transform.GetChild(0).gameObject;
        specCam = controller.transform.GetChild(2).gameObject;
        specCam.transform.position = new Vector3(-87,-5, -20);
    }

    void Update(){
        if (spikeScript != null)
        {
            if (pv.IsMine && spikeScript.defusing)
            {
                spikeScript.defuseText.text = "defusing...";
            }
            else if (spikeScript.defusing == false && pv.IsMine)
            {
                spikeScript.defuseText.text = "";
            }

            if (specCam != null && mCam != null)
            {
                if (spectate == true)
                {
                    mCam.SetActive(false);
                    specCam.SetActive(true);
                }
                else if (spectate == false)
                {
                    mCam.SetActive(true);
                    specCam.SetActive(false);
                }
            }
        }
    }

    
    public void CreateController(){
        spectate = false;
        Player player = PhotonNetwork.LocalPlayer; // or replace with the desired player object
        object teamObj = player.CustomProperties[TEAM_PROPERTY_KEY];
        int team = (int)teamObj;
        Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint(team);
        if (player.CustomProperties.ContainsKey(TEAM_PROPERTY_KEY))
        {
            if (team == 0) {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NeonContainer"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID });
            } else if (team == 1) {
                controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "JettContainer"), spawnpoint.position, spawnpoint.rotation, 0, new object[] {pv.ViewID });
            } else {
                Debug.LogError("Erorr: No team assigned");
            }
        }
        SpectateCam();  
    }


    [PunRPC]
    public void RPC_CreateController()
    {
        if (pv.IsMine)
        {
            PhotonNetwork.Destroy(controller);
            controller.SetActive(false);
            spectate = false;
            Player player = PhotonNetwork.LocalPlayer; // or replace with the desired player object
            object teamObj = player.CustomProperties[TEAM_PROPERTY_KEY];
            int team = (int)teamObj;
            Transform spawnpoint = SpawnManager.Instance.GetSpawnPoint(team);
            if (player.CustomProperties.ContainsKey(TEAM_PROPERTY_KEY))
            {
                if (team == 0)
                {
                    controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "NeonContainer"),
                        spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
                }
                else if (team == 1)
                {
                    controller = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "JettContainer"),
                        spawnpoint.position, spawnpoint.rotation, 0, new object[] { pv.ViewID });
                }
                else
                {
                    Debug.LogError("Erorr: No team assigned");
                }
            }
    
            SpectateCam();
        }
    }

    [PunRPC]
    public void RPC_UpdateDeadBluePlayers()
    {
        manager.deadBlueTeamPlayers += 1;
    }

    [PunRPC]
    public void RPC_UpdateDeadRedPlayers()
    {
        manager.deadRedTeamPlayers += 1;
    }

    public void Die(){
        if (controller == null){
            return;
        }
        Debug.Log("died");
        if (controller.transform.GetChild(1).GetComponent<Movement>().team == 0){
            pv.RPC("RPC_UpdateDeadBluePlayers", RpcTarget.All);
        } else if (controller.transform.GetChild(1).GetComponent<Movement>().team == 1){
            pv.RPC("RPC_UpdateDeadRedPlayers", RpcTarget.All);
        }
        PhotonNetwork.Destroy(controller.transform.GetChild(1).gameObject);
        controller.transform.GetChild(1).gameObject.SetActive(false);
        spectate = true;
        
        deaths++;

        Hashtable hash = new Hashtable();
        hash.Add("deaths", deaths);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    
    public void InstantiateKillFeedMessage(string killer, string killed){
        Debug.Log("Instantiating kill message: " + killer + " " + "brutally murdered" + " " + killed);
        GameObject killFeedItem = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs","KillFeedItem"), Vector3.zero, Quaternion.identity);
        killFeedItem.GetComponent<KillFeedItem>().SetUp(killer, killed);
        pv.RPC("RPC_SetKillMessageParent", RpcTarget.All);
    }

    [PunRPC]
    void RPC_SetKillMessageParent(){
        Debug.Log("controller");
        killFeedItem.transform.SetParent(killFeedContent);
    }

    public void GetKill(){
        pv.RPC(nameof(RPC_GetKill), pv.Owner);
    }

    public void StartRound(){
        return;
    }

    public void RunRPC()
    {
        pv.RPC("RPC_CreateController", RpcTarget.All);
    }


    [PunRPC]
    void RPC_GetKill(){
        kills++;

        Hashtable hash = new Hashtable();
        hash.Add("kills", kills);
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    public static PlayerManager Find(Player player){
        return FindObjectsOfType<PlayerManager>().SingleOrDefault(x => x.pv.Owner == player);
    }

}
