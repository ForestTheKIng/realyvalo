using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using System.IO;

public class SingleShotGun : Gun
{
    public Rigidbody2D _rigidbody;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private Animator _muzzleFlashAnimator;
    [SerializeField] private GameObject _bulletTrail;

    PlayerManager playerManager;


    PhotonView pv;

    void Awake() {
        pv = GetComponent<PhotonView>();    
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();    
    }

    public override void Use(){
        Shoot();
    }

    void Shoot(){
        _muzzleFlashAnimator.SetTrigger("Shoot");

        var hit = Physics2D.Raycast(_gunPoint.position, transform.up, (((GunInfo)itemInfo)._weaponRange));

        var trail = Instantiate(_bulletTrail, _gunPoint.position, transform.rotation);


        Destroy(trail, 0.5f);
        var trailScript = trail.GetComponent<BulletTrail>();

        if (hit.collider != null){
            IDamageable idamageable = hit.collider.gameObject.GetComponent<IDamageable>();

            trailScript.SetTargetPosition(hit.point);
            if (idamageable != null){
                idamageable.TakeDamage(((GunInfo)itemInfo).damage);
            }
            if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject.GetComponent<Movement>().currentHealth <= 0){
                playerManager.InstantiateKillFeedMessage(PhotonNetwork.NickName, hit.collider.gameObject.GetComponent<PhotonView>().Owner.NickName);
            }
            pv.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        } else {
            var endPosition = _gunPoint.position + transform.up * ((GunInfo)itemInfo)._weaponRange; 
            trailScript.SetTargetPosition(endPosition);

        }
    
    }

    [PunRPC]
    void RPC_Shoot(Vector2 hitPosition, Vector2 hitNormal){
        Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
        GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition, Quaternion.identity * bulletImpactPrefab.transform.rotation);  
        Destroy(bulletImpactObj, 5f);
    }
}
