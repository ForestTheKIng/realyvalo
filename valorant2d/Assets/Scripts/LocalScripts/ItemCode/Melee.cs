using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public abstract class Melee : Item
{
    public abstract override void Use();
}