using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    // There be singletons in these waters. Well, one because y'know, singleton.
    public static gameManager instance;

    public GameObject player;
    public GameObject enemy;
    public GameObject friendly;

    //public playerController playerScript;

    public GameObject pauseMenu;
    public GameObject debugMenu; // child of pauseMenu
    public GameObject statsMenu; // Child of pauseMenu
    public GameObject mainMenu; // may become separate scene and change functionality

    public GameObject playerDamage; // panel that provides feedback when player is hurt

    public bool isPaused; // if game is paused
    float timeScaleOrig; // global timescale variable

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

        //Time Scale
        timeScaleOrig = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        // if escape button is pressed
        if (Input.GetButtonDown("Cancel"))
        {
            //makes isPaused a toggle
            isPaused = !isPaused;

            //set active is how you turn game objects on and off
            pauseMenu.SetActive(isPaused);

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

        // Turn off pause menu
        pauseMenu.SetActive(false);
    }
}
