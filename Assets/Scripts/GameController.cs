using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    
    public float score;
    public float time;
    public float scorePerTank;
    public float volumeLevel;    // REAL MIXER INSTEAD OF THIS??
    public int lives;
    public int enemyStartingAmount;
    public int maxEnemiesAmount;
    public UIController ui;
    public AudioSource audioSource;
    public Spawner spawner;
    public GameObject forceField;
    
    private int currentLives;
    private int currentEnemyAmount;
    private GameObject player;
    private Health playerHealth;
    private PlayerControls playerControls;

    public static GameController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // enemy spawns
        for (int i = 0; i < enemyStartingAmount; i++)
        {
            spawner.SpawnEnemy();
        }
        
        // player spawn
        player = spawner.SpawnPlayer();
        playerHealth = player.GetComponent<Health>();
        playerControls = player.GetComponent<PlayerControls>();
        score = 0f;
        time = 0;
        currentLives = lives;
        currentEnemyAmount = enemyStartingAmount;
        forceField = GameObject.FindWithTag("Force_Field");
        forceField.SetActive(false);

        // set UI
        ui.setScore(score);
        ui.setTime(time);
        ui.setMechs(currentLives, lives);
        
        // for master volume
        AudioListener.volume = volumeLevel;
    }
    
    void Update()
    {
        playerHealth.DeathCheck();    // first things first =)

        // game timer
        if (currentLives >= 0 && player != null)
        {
            time += Time.deltaTime;
            ui.setTime((int) time);    
        }
        
        if (player == null)    // player has died
        { 
            if (currentLives > 0)
            {
                ui.ShowRespawnScreen();
                
                if (Input.GetButtonDown("Restart"))    // button space
                {
                    // player respawn --> set defaults
                    ui.HideRespawnScreen();
                    player = spawner.SpawnPlayer();
                    playerHealth = player.GetComponent<Health>();
                    playerControls = player.GetComponent<PlayerControls>();
                    currentLives--;
                    forceField = GameObject.FindWithTag("Force_Field");
                    forceField.SetActive(false);
                    ui.toggleAutoCannon();
                    ui.setMechs(currentLives, lives);
                }
            }
            else 
            {
                // game over 
                ui.ShowEndScreen(score, time);
            }
        }
        
        // player controls
        if (Input.GetButton("Autocannon"))    // button 1
        {
            playerControls.weapon = PlayerControls.Weapon.Autocannon;
            playerControls.weaponIndex = 0;
            ui.toggleAutoCannon();
        }
        else if (Input.GetButton("Missiles"))    // button 2
        {
            playerControls.weapon = PlayerControls.Weapon.Missiles;
            playerControls.weaponIndex = 1;
            ui.toggleMissiles();
        }
        else if (Input.GetButton("Energy beam"))    // button 3
        {
            playerControls.weapon = PlayerControls.Weapon.Beam;
            playerControls.weaponIndex = 2;
            ui.toggleBeam();
        }
        else if (Input.GetButtonDown("Shields"))    // button F
        {
            if (!playerHealth.shieldsOn && playerHealth.GetCurrentPower() > 0)
            {
                playerHealth.shieldsOn = true;
                ui.shieldsOn();
                forceField.SetActive(true);
            }
            else
            {
                playerHealth.shieldsOn = false;
                ui.shieldsOff();
                forceField.SetActive(false);
            }
        }
        
        // heat damage is reduced over time
        if (playerHealth.GetCurrentHeat() > 0)
        {
            playerHealth.AddHeat(-playerHealth.heatCoolingRate * Time.deltaTime);    
            SetHeat(playerHealth.GetCurrentHeat(), playerHealth.maxHeat);
        }
        
        // shields being on drains power
        if (playerHealth.GetCurrentPower() > 0 && playerHealth.shieldsOn)
        {
            playerHealth.ReducePower(playerHealth.heatCoolingRate * Time.deltaTime);    
            SetPower(playerHealth.GetCurrentPower(), playerHealth.maxPower);
        }
    }

    public void EnemyDestroyed()
    {
        spawner.SpawnEnemy();
        score += scorePerTank;
        ui.setScore(score);
        
        if (currentEnemyAmount < maxEnemiesAmount)
        {
            spawner.SpawnEnemy();
            currentEnemyAmount++;
        }
    }

    public void SetHealth(float current, float max)
    {
        if (current < 0)
        {
            current = 0f;
        }
        ui.setHealth(current);
    }

    public void SetPower(float current, float max)
    {
        if (current < 0)
        {
            current = 0f;
        }
        ui.setPower(current);
    }
    
    public void SetHeat(float current, float max)
    {
        if (current < 0)
        {
            current = 0f;
        }
        ui.setHeat(current);
    }
    
    public void SetShields(bool value)
    {
        playerHealth.shieldsOn = value;
        if (value)
        {
            ui.shieldsOn();
            forceField.SetActive(true);
        }
            
        else
        {
            ui.shieldsOff();
            forceField.SetActive(false);
        }
            
    }

    public bool CheckOkToShoot(PlayerControls.Weapon weapon)
    {
        switch (weapon)
        {
            case PlayerControls.Weapon.Autocannon:
                return true;
            case PlayerControls.Weapon.Missiles:
                if (playerControls.currentAmmo >= 1)
                {
                    playerControls.currentAmmo--;
                    ui.setAmmo(playerControls.currentAmmo);
                    return true;
                }
                break;
            case PlayerControls.Weapon.Beam:
                if (playerHealth.GetCurrentPower() >= 20 && !playerHealth.shieldsOn)    // energy beam cannot be used while shields on
                {
                    playerHealth.ReducePower(20);
                    playerHealth.AddHeat(10);
                    ui.setPower(playerHealth.GetCurrentPower());
                    ui.setHeat(playerHealth.GetCurrentHeat());
                    return true;
                }
                break;
        }

        return false;
    }

    public GameObject getPlayer()
    {
        return player;
    }


    
    

}
