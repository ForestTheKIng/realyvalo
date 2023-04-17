using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public abstract class Gun : Item
{
    public abstract override void Use();
    public GameObject bulletImpactPrefab;
}
