using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class medkitPickup : MonoBehaviour
{
    [SerializeField] medkitStats medkit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.healthPickup(medkit);
            Destroy(gameObject);
        }
    }
}
