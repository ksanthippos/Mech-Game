using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    // HUD 
    public Text scoreText;
    public Text timeText;
    public Text healthText;
    public Text mechsText;
    public Text heatText;
    public Text powerText;
    public Text ammoText;
    public Text autoCannonText;
    public Text missilesText;
    public Text beamText;
    public Text shieldsText;

    public GameObject healthBar;
    public GameObject powerBar;
    public GameObject heatBar;
    public GameObject ammoBar;

    // end screen
    public Text EndScoreText;
    public Text endTimeText;
    
    // menus
    public GameObject pauseMenu;
    public GameObject respawnScreen;
    public GameObject endScreen;

    private Slider healthSlider;
    private Slider powerSlider;
    private Slider heatSlider;
    private Slider ammoSlider;

    public SoundManager soundManager;
    
    private void Start()
    {
        toggleAutoCannon();
        shieldsOff();

        healthSlider = healthBar.GetComponent<Slider>();
        powerSlider = powerBar.GetComponent<Slider>();
        heatSlider = heatBar.GetComponent<Slider>();
        ammoSlider = ammoBar.GetComponent<Slider>();
        soundManager = GetComponent<SoundManager>();
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

    public void setTime(float time)
    {
        timeText.text = "Time: " + time;
    }

    public void setHealth(float value)
    {
        healthText.text = "Health: " + (int) value + "%";
        healthSlider.value = value;
        
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
        powerSlider.value = value;
        
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
        heatSlider.value = value;
        
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
        ammoSlider.value = value;
        
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
        autoCannonText.color = Color.green;
        missilesText.color = Color.gray;
        beamText.color = Color.grey;
        soundManager.PlayWeaponsChange();
    }

    public void toggleMissiles()
    {
        autoCannonText.color = Color.gray;
        missilesText.color = Color.green;
        beamText.color = Color.grey;
        soundManager.PlayWeaponsChange();
    }

    public void toggleBeam()
    {
        autoCannonText.color = Color.gray;
        missilesText.color = Color.gray;
        beamText.color = Color.green;
        soundManager.PlayWeaponsChange();
    }

    public void shieldsOn()
    {
        shieldsText.text = "ON";
        shieldsText.color = Color.green;
        soundManager.PlayShieldsUp();   
    }
    
    public void shieldsOff()
    {
        shieldsText.text = "OFF";
        shieldsText.color = Color.gray;
        soundManager.PlayShieldsDown();
    }
    

    public void togglePause()
    {
        soundManager.PlayPauseSound();
        
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

    public void ShowEndScreen(float score, float time)
    {
        EndScoreText.text = "Total score: " + score + " points";
        endTimeText.text = "Survived: " + (int) time + " seconds";
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
