using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    public Text scoreText;
    public Text healthText;
    public Text livesText;
    public Text EndScoreText;
    
    public GameObject pauseMenu;
    public GameObject respawnScreen;
    public GameObject endScreen;


    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            togglePause();
        }
    }

    public void setScore(float score)
    {
        scoreText.text = "Score: " + score;
    }

    public void setHealth(float current, float max)
    {
        healthText.text = "Health: " + current + "/" + max;
        if (current <= 25)
        {
            healthText.color = Color.red;
        }
        else
        {
            healthText.color = Color.white;
        }
    }

    public void setLives(int current, int max)
    {
        livesText.text = "Lives: " + current + "/" + max;
        if (current == 0)
        {
            livesText.color = Color.red;
        }
        else
        {
            livesText.color = Color.white;
        }
    }

    public void togglePause()
    {
        if (pauseMenu.activeInHierarchy)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;    // normal time
        }
        else if (!endScreen.activeInHierarchy && !respawnScreen.activeInHierarchy)
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;     // time is stopped
        }
    }

    public void ShowRespawnScreen()
    {
        respawnScreen.SetActive(true);
    }
    
    public void HideRespawnScreen()
    {
        respawnScreen.SetActive(false);
    }

    public void ShowEndScreen(float score)
    {
        EndScoreText.text = "Your score: " + score;
        endScreen.SetActive(true);
    }

    public void Restart()
    {
        // gets the current scene and starts it over
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // quitting possible in two scenarios: in editor or in ready-built-game
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
