using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class abyssalNet : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.collider.gameObject.transform.position = gameManager.instance.playerSpawnPos.transform.position;
            gameManager.instance.playerScript.takeDamage(10);
        }
    }
}
