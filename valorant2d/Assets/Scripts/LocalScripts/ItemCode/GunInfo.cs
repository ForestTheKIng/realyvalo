using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FPS/New Gun")]
public class GunInfo : ItemInfo
{
    public float damage;
    public float weaponRange;
    public float fireRate;
    public int maxAmmo;
    public int ammo;
    public float reloadSpeed;
    public float moveSpeed;
}
