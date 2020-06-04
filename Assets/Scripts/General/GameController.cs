using System;
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
    public float volumeLevel;
    
    private int currentLives;
    private int currentEnemyAmount;
    
    private GameObject player;

    public static GameController instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        for (int i = 0; i < enemyStartingAmount; i++)
        {
            spawner.SpawnEnemy();
        }
        
        player = spawner.SpawnPlayer();
        audioSource.Play();
        
        score = 0f;
        currentLives = lives;
        currentEnemyAmount = enemyStartingAmount;
        
        // set UI
        ui.setScore(score);
        ui.setLives(currentLives, lives);

        AudioListener.volume = volumeLevel;

    }
    
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        { 
            if (currentLives > 0)
            {
                ui.ShowRespawnScreen();
                
                if (Input.GetButtonDown("Restart"))
                {
                    ui.HideRespawnScreen();
                    player = spawner.SpawnPlayer();
                    audioSource.Play();
                    currentLives--;
                    ui.setLives(currentLives, lives);
                }
            }
            else 
            {
                ui.ShowEndScreen(score);
            }
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
        ui.setHealth(current, max);
    }
    

}
