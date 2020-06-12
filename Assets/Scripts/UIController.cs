using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    // HUD 
    public Text scoreText;
    public Text healthText;
    public Text mechsText;
    public Text heatText;
    public Text powerText;
    public Text ammoText;
    public Text autoCannonText;
    public Text missilesText;
    public Text beamText;
    public Text shieldsText;
    
    // end screen
    public Text EndScoreText;
    
    // menus
    public GameObject pauseMenu;
    public GameObject respawnScreen;
    public GameObject endScreen;
    

    private void Start()
    {
        toggleAutoCannon();
        shieldsOff();
        
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            togglePause();
        }
    }

    // HUD controls
    public void setScore(float score)
    {
        scoreText.text = "Score: " + score;
    }

    public void setHealth(float value)
    {
        healthText.text = "Health: " + (int) value + "%";
        if (value <= 25)
        {
            healthText.color = Color.red;
        }
        else
        {
            healthText.color = Color.white;
        }
    }
    
    public void setPower(float value)
    {
        powerText.text = "Power: " + (int) value + "%";
        if (value <= 25)
        {
            powerText.color = Color.red;
        }
        else
        {
            powerText.color = Color.white;
        }
    }

    public void setHeat(float value)
    {
        heatText.text = "Heat: " + (int) value + "%";
        if (value >= 75)
        {
            heatText.color = Color.red;
        }
        else
        {
            heatText.color = Color.white;
        }
    }

    public void setAmmo(int value)
    {
        ammoText.text = "Ammo: " + value;
        if (value < 2)
        {
            ammoText.color = Color.red;
        }
        else
        {
            ammoText.color = Color.white;
        }
    }
    
    public void setMechs(int current, int max)
    {
        mechsText.text = "Mechs: " + current + "/" + max;
        if (current == 0)
        {
            mechsText.color = Color.red;
        }
        else
        {
            mechsText.color = Color.white;
        }
    }

    public void toggleAutoCannon()
    {
        autoCannonText.color = Color.white;
        missilesText.color = Color.gray;
        beamText.color = Color.grey;
    }

    public void toggleMissiles()
    {
        autoCannonText.color = Color.gray;
        missilesText.color = Color.white;
        beamText.color = Color.grey;
    }

    public void toggleBeam()
    {
        autoCannonText.color = Color.gray;
        missilesText.color = Color.gray;
        beamText.color = Color.white;
    }

    public void shieldsOn()
    {
        shieldsText.text = "Shields: ON";
        shieldsText.color = Color.green;
    }
    
    public void shieldsOff()
    {
        shieldsText.text = "Shields: OFF";
        shieldsText.color = Color.gray;
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
