using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]


public class gunStats : ScriptableObject
{
    public GameObject model;
    public GameObject hitEffect;
    public AudioClip sound;
   
    public float shootRate;
    public int shootDist;
    public int shootDamage;
}
