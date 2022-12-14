using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class gameManager : MonoBehaviour
{
    // There be singletons in these waters. Well, one because y'know, singleton.
    public static gameManager instance;

    [Tooltip("Player object goes here")]public GameObject player;
    [Tooltip("Enemy game object here")] public GameObject enemy;
    [Tooltip("Boss GameObject here")] public GameObject boss;
    [Tooltip("Player Spawn Pos obj goes here.")] public GameObject playerSpawnPos;

    public bossAI bossScript;
    [Tooltip("Player script here")] public playerController playerScript;

    [Tooltip("Temp to hold currently shown menu.")] public GameObject menuCurrentlyOpen; // holds currently used menu
    [Tooltip("Pause menu object goes here")] public GameObject pauseMenu;   
    [Tooltip("Game Over menu object goes here")] public GameObject playerDeadMenu;
    [Tooltip("Win Menu object goes here")] public GameObject winMenu;
    [Tooltip("Start Message object goes here")] public GameObject startMessage;
    
    [Tooltip("Player Damage object goes here")] public GameObject playerDamage; // panel that provides feedback when player is hurt

    [Tooltip("HPBar under HPFrame goes here")] public Image HPBar;
    [Tooltip("Enemy Counter object goes here")] public TextMeshProUGUI enemyUICounter;


    public bool isPaused; // if game is paused
    public bool playerDead;
    public bool bossDead;


    float timeScaleOrig; // global timescale variable
    public int enemyCount;
    public int enemyCountMax;
    public int enemiesKilled;


    // Awake to be avail before any start
    void Awake()
    {
        instance = this;

        //Set's player
        player = GameObject.FindWithTag("Player"); //tags will need to be assigned to their respective objects in unity editor

        //Get player script here
        playerScript = player.GetComponent<playerController>();

        //set enemy
        enemy = GameObject.FindWithTag("Enemy"); //tags will need to be assigned to their respective objects in unity editor
        //set Boss
        boss = GameObject.FindWithTag("Boss");        

        //find and set spawn point
        playerSpawnPos = GameObject.Find("Player Spawn Pos");

        StartCoroutine(displayStartMessage());

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
        //enemiesKilled++;
        enemyUICounter.text = enemyCount.ToString("F0");

        if (enemyCount <= 0)
        {
            StartCoroutine(displayWin());
        }
        //if (enemyCountMax == enemiesKilled)
        //{
        //    StartCoroutine(displayWin());
        //}
    }

    public void enemyIncrement(int num)
    {
        enemyCount += num;
        //if (num == 0)
        //{
        //    enemyCount++;
        //}
        enemyUICounter.text = enemyCount.ToString("F0");
    }

    IEnumerator displayWin()
    {
        yield return new WaitForSeconds(2);

        menuCurrentlyOpen = winMenu;
        menuCurrentlyOpen.SetActive(true);
        cursorLockPause();
    }

    IEnumerator displayStartMessage()
    {
        startMessage.SetActive(true);
        yield return new WaitForSeconds(3);
        startMessage.SetActive(false);
    }
}
