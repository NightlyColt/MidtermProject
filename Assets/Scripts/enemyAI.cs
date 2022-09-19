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
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int FOV;


    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    Vector3 playerDir;
    Vector3 lastPlayerPos;
    Vector3 startingPos;

    float stoppingDistanceOrig;
    float speedOrig;
    bool playerInRange;
    bool isShooting;
    float angleLook;
    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemyIncrement();
        lastPlayerPos = transform.position;
        stoppingDistanceOrig = agent.stoppingDistance;
        speedOrig = agent.speed;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        angleLook = Vector3.Angle(playerDir, transform.forward);

        if (playerInRange)
        {
            canSeePlayer();
        }
        if (agent.remainingDistance < 0.001f)
        {
            roam();
        }

        playerDir = gameManager.instance.player.transform.position - transform.position;

    }

    void roam()
    {
        agent.stoppingDistance = 0;
        agent.speed = speedOrig;

        Vector3 randomDir = Random.insideUnitSphere * roamRadius;
        randomDir += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomDir, out hit, 5, 5);
        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(hit.position, path);
        agent.SetPath(path);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("Enter field");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            lastPlayerPos = gameManager.instance.player.transform.position;
            agent.stoppingDistance = 0;
            Debug.Log("Exit field");
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
            Debug.DrawRay(transform.position, playerDir);
            if (rayHit.collider.CompareTag("Player") && angleLook <= FOV)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistanceOrig;

                facePlayer();

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
            else
                agent.stoppingDistance = 0;
        }
        if (gameManager.instance.playerDeadMenu.activeSelf)
        {
            agent.stoppingDistance = 0;
            playerInRange = false;
        }
    }
} 

