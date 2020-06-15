using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public float playerX;
    public float playerZ;
    
    public float range;
    public float spawnHeight;
    public float spawnOffSet;
    
    public GameObject player;
    public GameObject enemy;
    public Collider area;

    private GameController gameController;
    
    public GameObject SpawnPlayer()
    {
        return Spawn(player);
    }

    public GameObject SpawnEnemy()
    {
        return Spawn(enemy);
    }
    
    private GameObject Spawn(GameObject obj)
    {
        
        Vector3 maxPoint = area.bounds.max;

        // player and enemies spawn at random location
        float randomx = Random.Range(spawnOffSet, maxPoint.x - spawnOffSet);
        float randomz = Random.Range(spawnOffSet, maxPoint.z - spawnOffSet);
    
        Vector3 position = new Vector3(randomx, spawnHeight, randomz);
        Collider[] colliders = Physics.OverlapSphere(position, range);    // same technique in explosions

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
