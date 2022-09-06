using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamageable
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer mesh;

    [SerializeField] int HP;
    [SerializeField] float lookSens;

    [SerializeField] GameObject bullet;
    [SerializeField] int shootRate;
    [SerializeField] Transform shootPos;

    Vector3 playerDir;
    bool isShooting;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        LookForPlayer();
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            StartShooting();
        }
    }
    /// <summary>
    /// This would look for the player's position and set it's destination to the player's direction
    /// </summary>
    void LookForPlayer()
    {
        // player's position - enemy's position = player's direction
        playerDir = gameManager.instance.player.transform.position - transform.position;
        agent.SetDestination(gameManager.instance.player.transform.position);
    }

    void StartShooting()
    {
        if (!isShooting)
        {
            StartCoroutine(shoot());
        }
        facePlayer();
    }
   
    void facePlayer()
    {
        // Don't need the AI to look upwards along with the body
        playerDir.y = 0;

        // Get the angle of the player's position
        Quaternion rotation = Quaternion.LookRotation(playerDir);

        // update the current rotation slightly to not make it look chopy
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * lookSens);
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;
        StartCoroutine(flashDamage());
        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        mesh.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        // Creates the bullet at the position of the gun and the direction of the enemy
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }
}
