using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class friendlyAI : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Renderer rend;
    NavMeshPath path;

    [Header("----- Friendly Stats -----")]
    [Range(0, 10)][SerializeField] int HP;
    [Range(0, 10)][SerializeField] int enemyFaceSpeed;
    [Range(0, 50)][SerializeField] float maxAggroDist; // range of pathing to enemy targets

    [Header("----- Weapon Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos; // allows us to set position of instantiated bullet

    Vector3 enemyDir;
    bool isShooting;
    float elapsedTime;

    // Start is called before the first frame update
    void Start()
    {
        path = new NavMeshPath();
    }

    // Update is called once per frame
    void Update()
    {        
        if (gameManager.instance.enemy != null)
        {
            // Tells AI to navigate to enemy
            agent.SetDestination(gameManager.instance.enemy.transform.position);

            enemyDir = gameManager.instance.enemy.transform.position - transform.position;
            //When AI is within stopping distance
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                faceEnemy();
            }
        }
    }
    public void takeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashColor());

        if (HP <= 0)
        {
            Destroy(gameObject);
        }
    }

    void faceEnemy()
    {
        // don't take enemy's y position into account
        enemyDir.y = 0;
        Quaternion rotation = Quaternion.LookRotation(enemyDir); // quaternion rotation calc for direction of enemy
        //delta time makes the speed based on time instead of framerate
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * enemyFaceSpeed); // lerp allows for a smooth transition to an end state float or direction
    }

    IEnumerator flashColor()
    {
        rend.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        rend.material.color = Color.white;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        //create bullet in real time
        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }
}
