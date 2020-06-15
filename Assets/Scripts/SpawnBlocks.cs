﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{

    public int amount;
    public float minSize;
    public float maxSize;
    public float spawnOffSet;
    
    public GameObject block;
    public Collider ground;
    
    
    void Start()
    {
        //Vector3 minPoint = ground.bounds.min;
        Vector3 maxPoint = ground.bounds.max;

        for (int i = 0; i < amount; i++)
        {

            float randomx = Random.Range(spawnOffSet, maxPoint.x - spawnOffSet);
            float randomz = Random.Range(spawnOffSet, maxPoint.z - spawnOffSet);
            float randomSize = Random.Range(minSize, maxSize);
            Vector3 position = new Vector3(randomx, 0.5f, randomz);
            GameObject spawnedBlock = Instantiate(block, position, new Quaternion(), transform);
            spawnedBlock.transform.localScale *= randomSize;
            //>spawnedBlock.GetComponent<Health>().maxHealth = randomSize * 25;
            
        }
    }

}
