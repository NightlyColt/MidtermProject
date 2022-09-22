using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class teleporter : MonoBehaviour
{
    [Tooltip("Interact message object goes here")] public GameObject interactMessageObj;
    [SerializeField] GameObject forceField;
    [SerializeField] List<spawner> spawners = new List<spawner>();
    bool inRange;
    bool activated;
    bool isScaling;
    float time;
    Vector3 playerDir;
    Vector3 scaleMax;
    Vector3 scaleMin;

    // Start is called before the first frame update
    void Start()
    {
        scaleMax = new Vector3(200f, 200f, 200f);
        scaleMin = new Vector3(1f, 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {



        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            activated = true;
            interactMessageObj.SetActive(false);
            foreach (spawner spawn in spawners)
            {
                spawn.startSpawning = true;
            }
            //set boss script
            gameManager.instance.bossScript = gameManager.instance.boss.GetComponent<bossAI>();

        }

        if (activated && forceField.transform.localScale != scaleMax && gameManager.instance.bossDead == false)
        {
            StartCoroutine(scaleUpWithTime());
        }

        if (gameManager.instance.bossDead == true && forceField.transform.localScale != scaleMin )
        {
            StartCoroutine(scaleDownWithTime());
        }

        if (interactMessageObj.activeInHierarchy)
        {
            playerDir = gameManager.instance.player.transform.position - transform.position;


            Quaternion rotation = Quaternion.LookRotation(playerDir);


            interactMessageObj.transform.rotation = rotation;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        inRange = true;
        if (!activated)
        {
            interactMessageObj.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        inRange = false;
        if (other.tag == "Player")
        {
            interactMessageObj.SetActive(false);
        }
    }

    IEnumerator scaleUpWithTime()
    {
        if (!isScaling)
        {
            isScaling = true;
            forceField.transform.localScale += new Vector3(1f, 1f, 1f);
            yield return new WaitForSeconds(Time.deltaTime);
            isScaling = false;
        }
        else
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public IEnumerator scaleDownWithTime()
    {
        if (!isScaling)
        {
            isScaling = true;
            forceField.transform.localScale -= new Vector3(1f, 1f, 1f);
            yield return new WaitForSeconds(Time.deltaTime);
            isScaling = false;
        }
        else
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
