using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [SerializeField] int timer;
    [Tooltip("Optional, after spawning the enemy will run here if it is set")][SerializeField] GameObject optionalDestination;
    int enemiesSpawned;
    bool isSpawning;
    public bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Update()
    {
        if (startSpawning)
        {
            StartCoroutine(spawn());
        }
    }

    IEnumerator spawn()
    {

        if (!isSpawning && enemiesSpawned < maxEnemies)
        {
            
            isSpawning = true;
            enemiesSpawned++;

            Instantiate(enemy, transform.position, enemy.transform.rotation);
            if (optionalDestination != null)
            {
                enemyAI temp = enemy.GetComponent<enemyAI>();
                temp.agent.SetDestination(optionalDestination.transform.position);
            }
            yield return new WaitForSeconds(timer);
            isSpawning = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }
}
