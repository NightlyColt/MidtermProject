using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [Range(1, 10)][SerializeField] int timer;
    [Tooltip("Optional, after spawning the enemy will run here if it is set")][SerializeField] GameObject optionalDestination;
    int enemiesSpawned;
    bool isSpawning;
    public bool startSpawning;

    // Start is called before the first frame update
    void Start()
    {
        gameManager.instance.enemyIncrement(maxEnemies);
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

            yield return new WaitForSeconds(timer * Time.deltaTime);
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



