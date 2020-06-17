using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    
    public float range;
    public float spawnHeight;
    public float spawnOffSet;
    
    public GameObject player;
    public GameObject[] enemies;
    public Collider area;


    public GameObject SpawnPlayer()
    {
        return Spawn(player);
    }

    public GameObject SpawnEnemy(int level, int maxLevel)
    {
        int randInt;
        if (level < maxLevel)
        {
            randInt = Random.Range(level, enemies.Length - (maxLevel - level));
            return Spawn(enemies[randInt]);
        }
        
        randInt = Random.Range(0, enemies.Length);
        return Spawn(enemies[randInt]);
    }
    
    private GameObject Spawn(GameObject obj)
    {
        Vector3 maxPoint = area.bounds.max;

        // generate random position
        float randomx = Random.Range(spawnOffSet, maxPoint.x - spawnOffSet);
        float randomz = Random.Range(spawnOffSet, maxPoint.z - spawnOffSet);
        Vector3 position = new Vector3(randomx, spawnHeight, randomz);
        
        // search for other colliders within a range 
        Collider[] colliders = Physics.OverlapSphere(position, range);    

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].CompareTag("Obstacle"))
            {
                Destroy(colliders[i]);    // remove block if inside spawn range
            }
        }
    
        GameObject newObj = Instantiate(obj, position, new Quaternion());
        return newObj;
        
    }
    
}
