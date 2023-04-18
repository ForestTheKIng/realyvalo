using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "FPS/New Melee")]
public class MeleeInfo : ItemInfo
{
    public float damage;
    public float slashRate;
    public float moveSpeed;
}