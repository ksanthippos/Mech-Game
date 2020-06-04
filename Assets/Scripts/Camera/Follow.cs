using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{

    public Transform target;    // what is followed
    public Vector3 offset;    // how far target is followed

    private GameObject player;

    
    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player");
        }
        else
        {
            target = player.transform;
        }
        transform.position = target.position + offset;
    }
}
