using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    // There be singletons in these waters. Well, one because y'know, singleton.
    public static gameManager instance;

    public GameObject player;
    public GameObject enemy;
    public GameObject friendly;
    public GameObject playerSpawnPos;


    public playerController playerScript;

    public GameObject menuCurrentlyOpen; // holds currently used menu
    public GameObject pauseMenu;
    //public GameObject debugMenu; // child of pauseMenu
    //public GameObject statsMenu; // Child of pauseMenu
    //public GameObject mainMenu; // may become separate scene and change functionality
    public GameObject playerDeadMenu;
    public GameObject winMenu;


    public GameObject playerDamage; // panel that provides feedback when player is hurt

    public Image HPBar;
    public TextMeshProUGUI enemyUICounter;


    public bool isPaused; // if game is paused
    public bool playerDead;

    float timeScaleOrig; // global timescale variable
    public int enemyCount;


    // Awake to be avail before any start
    void Awake()
    {
        instance = this;

        //Set's player
        player = GameObject.FindWithTag("Player"); //tags will need to be assigned to their respective objects in unity editor

        //set enemy
        enemy = GameObject.FindWithTag("Enemy"); //tags will need to be assigned to their respective objects in unity editor

        //set friendly
        friendly = GameObject.FindWithTag("Friendly"); //tags will need to be assigned to their respective objects in unity editor

        //Get player script here
        playerScript = player.GetComponent<playerController>();

        //find and set spawn point
        playerSpawnPos = GameObject.Find("Player Spawn Pos");

        //Time Scale
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if cancel button is pressed
        if (Input.GetButtonDown("Cancel") && menuCurrentlyOpen != playerDeadMenu && menuCurrentlyOpen != winMenu)
        {
            //makes isPaused a toggle
            isPaused = !isPaused;

            //use menu placeholder
            menuCurrentlyOpen = pauseMenu;

            //set active is how you turn game objects on and off
            menuCurrentlyOpen.SetActive(isPaused);

            if (isPaused)
            {
                cursorLockPause();
            }
            else
            {
                cursorUnlockUnpause();
            }
        }
    }


    /// <summary>
    /// Pauses all game activity and displays cursor
    /// </summary>
    public void cursorLockPause()
    {
        //show cursor
        Cursor.visible = true;

        // confines cursor to screen but moveable
        Cursor.lockState = CursorLockMode.Confined;

        // Makes the whole system pauseo outside of UI update
        Time.timeScale = 0;
    }

    /// <summary>
    /// Hide cursor and lock it to center of screen. Resume all game activity. 
    /// </summary>
    public void cursorUnlockUnpause()
    {
        //hide cursor
        Cursor.visible = false;

        // confines cursor to middle of screen
        Cursor.lockState = CursorLockMode.Locked;

        // Makes the whole system resume
        Time.timeScale = timeScaleOrig;

        if (menuCurrentlyOpen != null)
        {
            // Turn off pause menu
            menuCurrentlyOpen.SetActive(false);
        }

        //clear current menu out to be reused
        menuCurrentlyOpen = null;
    }

    public void playerIsDead()
    {
        playerDead = true;

        //show player dead menu
        playerDeadMenu.SetActive(true);

        //assign to placeholder
        menuCurrentlyOpen = playerDeadMenu;

        playerDamage.SetActive(false);

        isPaused = true;

        //lock in
        cursorLockPause();        
    }

    public void enemyDecrement()
    {
        enemyCount--;
        enemyUICounter.text = enemyCount.ToString("F0");
        if (enemyCount <= 0)
        {
            StartCoroutine(displayWin());
        }
    }

    public void enemyIncrement()
    {
        enemyCount++;
        enemyUICounter.text = enemyCount.ToString("F0");
    }

    IEnumerator displayWin()
    {
        yield return new WaitForSeconds(2);

        menuCurrentlyOpen = winMenu;
        menuCurrentlyOpen.SetActive(true);
        cursorLockPause();
    }
}
