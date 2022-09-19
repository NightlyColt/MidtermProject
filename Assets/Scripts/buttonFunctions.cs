using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class buttonFunctions : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene(1); 
    }

    public void resume()
    {
        if (gameManager.instance.isPaused)
        {
            gameManager.instance.isPaused = !gameManager.instance.isPaused;
            gameManager.instance.cursorUnlockUnpause();
        }
    }

    public void restart()
    {
        gameManager.instance.cursorUnlockUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void playerRespawn()
    {
        gameManager.instance.playerScript.respawn();
    }

    public void exitGameQuit()
    {
        Application.Quit();
    }

    public void quit()
    {
        SceneManager.LoadScene(0);
    }



}