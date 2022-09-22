using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

public class gunStats : ScriptableObject
{
    public float shootRate;
    public int shootDist;
    public int shootDamage;
    public int ammoCount;
    public int magSize;
    public GameObject gunModel;
    public string gunName;
    public AudioClip gunSound;
}
