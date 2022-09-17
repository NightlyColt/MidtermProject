using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class teleporter : MonoBehaviour
{
    [Tooltip("Interact message object goes here")] public GameObject interactMessageObj;
    [SerializeField] GameObject forceField;
    bool inRange;
    bool activated;
    bool isScaling;
    float time;
    Vector3 playerDir;
    Vector3 scaleMax;

    // Start is called before the first frame update
    void Start()
    {
        scaleMax = new Vector3(99f, 99f, 99f);
    }

    // Update is called once per frame
    void Update()
    {

        if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            activated = true;
            forceField.SetActive(true);
            interactMessageObj.SetActive(false);
        }

        if (activated && forceField.transform.localScale != scaleMax)
        {
            StartCoroutine(scaleWithTime());
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

    IEnumerator scaleWithTime()
    {
        if (!isScaling)
        {
            isScaling = true;
            forceField.transform.localScale += new Vector3(1f, 1f, 1f);
            yield return new WaitForSeconds(Time.deltaTime/5);
            isScaling = false;
        }
        else
        {
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }
}
