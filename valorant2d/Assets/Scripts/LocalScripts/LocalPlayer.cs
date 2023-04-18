using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using UnityEngine.UI;
using System.IO;
using System.Linq;
using Unity.Mathematics;
using TMPro;

public class LocalPlayer : MonoBehaviourPunCallbacks, IDamageable
{
    private const string _TEAM_PROPERTY_KEY = "team";
    public PlayerManager playerManager;
    [NonSerialized] public float MoveSpeed;
    [SerializeField] Image healthbarImage;
    [SerializeField] GameObject ui;
    public Rigidbody2D rb;
    public Camera cam;
    Vector2 movement;
    Vector2 mousePos;
    PhotonView pv;
    public Jett jett;
    public Item[] items;
    public int itemIndex;
    int previousItemIndex = -1;
    const float maxHealth = 100f;
    public float currentHealth = maxHealth;
    public int team;
    public TMP_Text ammoText;
    public TMP_Text reloadText;


    void Awake() 
    {
        pv = GetComponent<PhotonView>();    
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();    
    }
    
    void Start(){
        if (pv.IsMine){
            EquipItem(0);
            Photon.Realtime.Player player = PhotonNetwork.LocalPlayer; // or replace with the desired player object
            if (player.CustomProperties.ContainsKey(_TEAM_PROPERTY_KEY))
            {
                object teamObj = player.CustomProperties[_TEAM_PROPERTY_KEY];
                team = (int)teamObj;
            } else {
                Debug.LogError("Erorr: No team assigned");
            }
        } else{
            rb.isKinematic = true;
            Destroy(ui);
        } 
    }



    // Update is called once per frame
    void Update()
    {
        Debug.Log(items);
        
        if(pv.IsMine)
        {
            movement.x = Input.GetAxisRaw("Horizontal");
            movement.y = Input.GetAxisRaw("Vertical");
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        for (int i = 0; i < items.Length; i++)
        {
            if (Input.GetKeyDown((i + 1).ToString()) && pv.IsMine){
                EquipItem(i);
                break;
            }
        }
        

        if (pv.IsMine){
            if (Input.GetMouseButtonDown(0)){
                items[itemIndex].Use();
            }

            if (Input.GetKeyDown("r"))
            {
                ((Gun)items[itemIndex]).Reload();
            }
        }

    }
    

    void EquipItem(int _index){

        if (_index == previousItemIndex){
            return;
        }

        itemIndex = _index;

        items[itemIndex].itemGameObject.SetActive(true);
        if (previousItemIndex != -1){
            items[previousItemIndex].itemGameObject.SetActive(false);
        }

        previousItemIndex = itemIndex;

        if (pv.IsMine){
            Hashtable hash = new Hashtable();

            hash.Add("itemIndex", itemIndex);
            PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
        }
    }

    public override void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, Hashtable changedProps){
        if (changedProps.ContainsKey("itemIndex") && !pv.IsMine && targetPlayer == pv.Owner){
            EquipItem((int)changedProps["itemIndex"]);
        }
    }
    private void FixedUpdate()
    {

        if (team == 0) {
            if(pv.IsMine)
            {
                rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime);

                Vector2 lookDir = mousePos - rb.position;
                float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;
            }
        } else if (team == 1) {
            if(pv.IsMine && jett.isDashing == false)
            {
                rb.MovePosition(rb.position + movement * MoveSpeed * Time.fixedDeltaTime);

                Vector2 lookDir = mousePos - rb.position;
                float angle = Mathf.Atan2(lookDir.y ,lookDir.x) * Mathf.Rad2Deg - 90f;
                rb.rotation = angle;
            }
        }

    }

    public void TakeDamage(float damage){
        pv.RPC(nameof(RPC_TakeDamage), pv.Owner, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(float damage, PhotonMessageInfo info){
        if (healthbarImage != null){
            currentHealth -= damage;
            healthbarImage.fillAmount = currentHealth / maxHealth;
        }

        if (currentHealth <= 0){
            Debug.Log("dead");
            Die();
            PlayerManager.Find(info.Sender).GetKill(); 
        }

    }

    void Die()
    {
        playerManager.Die();
    }
}


