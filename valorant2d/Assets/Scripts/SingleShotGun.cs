using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using System.IO;
using Unity.VisualScripting;

public class SingleShotGun : Gun
{
    public Rigidbody2D _rigidbody;
    [SerializeField] private Transform _gunPoint;
    [SerializeField] private Animator _muzzleFlashAnimator;
    [SerializeField] private GameObject _bulletTrail;

    PlayerManager playerManager;
    private bool _onCooldown;
    private LocalPlayer _myLp;

    PhotonView pv;

    void Awake()
    {
        ((GunInfo)itemInfo).maxAmmo = ((GunInfo)itemInfo).ammo;
        pv = GetComponent<PhotonView>();    
        playerManager = PhotonView.Find((int)pv.InstantiationData[0]).GetComponent<PlayerManager>();    
    }

    private void Update()
    {
        LocalPlayer[] localPlayers = FindObjectsOfType<LocalPlayer>();
        foreach (LocalPlayer localPlayer in localPlayers)
        {
            if (pv.IsMine)
            {
                _myLp = localPlayer;
                break;
            }
        }

        if (pv.IsMine)
        {
            _myLp.ammoText.text = ((GunInfo)itemInfo).ammo.ToString() + "/" + ((GunInfo)itemInfo).maxAmmo.ToString();
        }
    }

    public override void Reload()
    {
        StartCoroutine(ReloadCooldown());
    }

    public override void Use(){
        if (!_onCooldown && ((GunInfo)itemInfo).ammo >= 0) 
        {
            Shoot();
        }
    }

    private IEnumerator ReloadCooldown()
    {
        yield return new WaitForSeconds(((GunInfo)itemInfo).reloadSpeed);
        ((GunInfo)itemInfo).ammo = ((GunInfo)itemInfo).maxAmmo;
    }

    private IEnumerator ShootCooldown()
    {
        _onCooldown = true;
        yield return new WaitForSeconds(((GunInfo)itemInfo).fireRate);
        _onCooldown = false;
    }

    void Shoot()
    {
        ((GunInfo)itemInfo).ammo -= 1;
        StartCoroutine(ShootCooldown());
        _muzzleFlashAnimator.SetTrigger("Shoot");

        var hit = Physics2D.Raycast(_gunPoint.position, transform.up, (((GunInfo)itemInfo).weaponRange));

        var trail = Instantiate(_bulletTrail, _gunPoint.position, transform.rotation);


        Destroy(trail, 0.5f);
        var trailScript = trail.GetComponent<BulletTrail>();

        if (hit.collider != null){
            IDamageable idamageable = hit.collider.gameObject.GetComponent<IDamageable>();
            LocalPlayer lp = hit.collider.gameObject.GetComponent<LocalPlayer>();


        
            trailScript.SetTargetPosition(hit.point);
            if (idamageable != null)
            {
                if (_myLp.team != lp.team)
                {
                    idamageable.TakeDamage(((GunInfo)itemInfo).damage);
                }
            }
            if (hit.collider.gameObject.tag == "Player" && hit.collider.gameObject.GetComponent<LocalPlayer>().currentHealth <= 0){
                playerManager.InstantiateKillFeedMessage(PhotonNetwork.NickName, hit.collider.gameObject.GetComponent<PhotonView>().Owner.NickName);
            }
            pv.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);
        } else {
            var endPosition = _gunPoint.position + transform.up * ((GunInfo)itemInfo).weaponRange; 
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
