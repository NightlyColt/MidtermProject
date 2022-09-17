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
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    Vector3 playerDir;
    Vector3 companionDir;
    Vector3 lastPlayerPos;

    bool playerInRange;
    bool isShooting;
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemyIncrement();
    }

    // Update is called once per frame
    void Update()
    {
        GetTarget();
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

    /// <summary>
    /// This would look for the friendly's position and set it's destination to the player's direction
    /// </summary>
    void LookForCompanion()
    {
        // friendly's position - enemy's position = player's direction
        companionDir = gameManager.instance.friendly.transform.position - transform.position;
        agent.SetDestination(gameManager.instance.friendly.transform.position);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            lastPlayerPos = gameManager.instance.player.transform.position;
            agent.stoppingDistance = 0;
        }
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
        lastPlayerPos = gameManager.instance.player.transform.position;
        agent.SetDestination(lastPlayerPos);

        if (HP <= 0)
        {
            gameManager.instance.enemyDecrement();
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

    void canSeePlayer()
    {
        RaycastHit rayHit;
        if (Physics.Raycast(transform.position, playerDir, out rayHit))
        {

        }
    }
}
