using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gunPickup : MonoBehaviour
{
    [SerializeField] gunStats gunstat;

    private void OnTriggerEnter(Collider other)
    {
        gameManager.instance.playerScript.gunPickup(gunstat);
        Destroy(gameObject);
    }
}
