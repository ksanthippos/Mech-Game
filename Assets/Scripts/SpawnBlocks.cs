using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlocks : MonoBehaviour
{

    public int amount;
    public float minSize;
    public float maxSize;
    public GameObject block;
    public Collider ground;
    
    // Start is called before the first frame update
    void Start()
    {
        Vector3 minPoint = ground.bounds.min;
        Vector3 maxPoint = ground.bounds.max;

        for (int i = 0; i < amount; i++)
        {
            float randomx = Random.Range(minPoint.x, maxPoint.x);
            float randomz = Random.Range(minPoint.z, maxPoint.z);
            float randomSize = Random.Range(minSize, maxSize);
            
            Vector3 position = new Vector3(randomx, maxPoint.y + randomSize / 2, randomz);
            GameObject spawnedBlock = Instantiate(block, position, new Quaternion(), transform);
            spawnedBlock.transform.localScale *= randomSize;
            spawnedBlock.GetComponent<Health>().maxHealth = randomSize * 25;

        }
    }

}
