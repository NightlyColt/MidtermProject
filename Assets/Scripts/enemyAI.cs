using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamageable
{
    [Header("Componet")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer mesh;
    [SerializeField] Animator anim;
    [SerializeField] Transform headPos;

    [Header("Stats")]
    [SerializeField] int HP;
    [SerializeField] float lookSens;
    [SerializeField] float stunTime;
    [Range(1, 50)] [SerializeField] float sprintVelocity;
    [Range(1, 50)] [SerializeField] int roamRadius;
    [Range(1, 180)] [SerializeField] int FOV;

    [Header("Weapon")]
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
    bool takingDamage;
    float destionDistance;
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
        if (agent.enabled)
        {
            //calculateSpeed();
            anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * 4));

            angleLook = Vector3.Angle(playerDir, transform.forward);
            if (!takingDamage)
            {
                if (playerInRange)
                {
                    canSeePlayer();
                }
                if (agent.remainingDistance < 0.001f)
                {
                    roam();
                    StartCoroutine(stop());
                }
            }

            playerDir = gameManager.instance.player.transform.position - headPos.transform.position;
        }

    }

    IEnumerator stop()
    {
        agent.speed = 0;
        yield return new WaitForSeconds(1.5f);
        agent.speed = speedOrig;
    }
    void roam()
    {
        anim.SetBool("Sprinting", false);
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

        anim.SetTrigger("Damage");

        StartCoroutine(flashDamage());

        lastPlayerPos = gameManager.instance.player.transform.position;
        agent.SetDestination(lastPlayerPos);


        if (HP <= 0 && agent.enabled)
        {
            enemyDeath();
        }
    }

    IEnumerator flashDamage()
    {
        takingDamage = true;
        agent.speed = 0;
        foreach (Material material in mesh.materials)
        {
            material.color = Color.red;
        }

        yield return new WaitForSeconds(stunTime);

        foreach (Material material in mesh.materials)
        {
            material.color = Color.white;
        }

        agent.speed = speedOrig;
        takingDamage = false;
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
        if (Physics.Raycast(headPos.transform.position, playerDir, out rayHit))
        {
#if UNITY_EDITOR
            Debug.DrawRay(headPos.transform.position, playerDir);
#endif
            if (rayHit.collider.CompareTag("Player") && angleLook <= FOV)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);
                agent.stoppingDistance = stoppingDistanceOrig;
                agent.speed = sprintVelocity;

                facePlayer();

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
            else
            {
                agent.stoppingDistance = 0;
            }
        }
        if (gameManager.instance.playerDeadMenu.activeSelf)
        {
            agent.stoppingDistance = 0;
            playerInRange = false;
        }
    }

    void enemyDeath()
    {
        anim.SetBool("Dead", true);
        gameManager.instance.enemyDecrement();
        agent.enabled = false;
        foreach(Collider col in GetComponents<Collider>())
        {
            col.enabled = false;
        }
    }
} 

