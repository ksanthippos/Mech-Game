﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Spawner spawner;
    public float score;
    public int lives;
    public float scorePerTank;
    public int enemyStartingAmount;
    public int maxEnemiesAmount;
    public UIController ui;
    public AudioSource audioSource;
    public float volumeLevel;    // REAL MIXER INSTEAD OF THIS

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
        currentLives = lives;
        currentEnemyAmount = enemyStartingAmount;

        // set UI
        ui.setScore(score);
        ui.setMechs(currentLives, lives);
        
        // spawning audio clip
        audioSource.Play();
        // adjust master volume
        AudioListener.volume = volumeLevel;
    }
    
    void Update()
    {
        playerHealth.DeathCheck();    // first things first =)
        
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
                    audioSource.Play();
                    currentLives--;
                    ui.toggleAutoCannon();
                    ui.setMechs(currentLives, lives);
                }
            }
            else 
            {
                // game over 
                ui.ShowEndScreen(score);
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
            }
            else
            {
                playerHealth.shieldsOn = false;
                ui.shieldsOff();
            }
        }
        
        // heat damage is reduced over time
        if (playerHealth.GetCurrentHeat() > 0)
        {
            playerHealth.AddHeat(-playerHealth.heatCoolingRate * Time.deltaTime);    
            SetHeat(playerHealth.GetCurrentHeat(), playerHealth.maxHeat);
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
            ui.shieldsOn();
        else
            ui.shieldsOff();
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


    
    

}
