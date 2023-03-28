using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerManager : MonoBehaviourPunCallbacks
{

    
    PhotonView pv;

    GameObject controller;

    int kills;
    int deaths;
    
    public GameObject killFeedItem;

    public int roundNumber;

    [SerializeField] GameObject killFeedItemPrefab;
    [SerializeField] Transform killFeedContent;
    public Timer manager;

    private const string TEAM_PROPERTY_KEY = "team";
    private bool spectate = false;
    public GameObject spectateCam;

    void Awake() {
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
        spectateCam = controller.transform.GetChild(2).gameObject;
        Debug.Log(spectateCam);
    }

    void Update(){
        if (spectate == true){
            spectateCam.SetActive(true);
        } else if (spectate == false){
            spectateCam.SetActive(false);
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

    public void Die(){
        if (controller == null){
            return;
        }
        if (controller.transform.GetChild(1).GetComponent<Movement>().team == 0){
            manager.deadBlueTeamPlayers += 1;
        } else if (controller.transform.GetChild(1).GetComponent<Movement>().team == 1){
            manager.deadRedTeamPlayers += 1;
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
