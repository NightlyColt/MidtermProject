using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerUp : MonoBehaviour
{
    [SerializeField] powerUpStats power;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.giveJump(power.jumpHeightAdded);
            Destroy(gameObject);
        }
    }
}
