using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed;
    public float time;
    public float radius;
    public float damage;

    public string shooterTag;
    public GameObject explosion;

    private Rigidbody rb;
    private float t;
    private AudioSource audioSource;
    
    // Start is called before the first frame update
    void Start()
    {
        t = time;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        rb.velocity = transform.forward * speed;
    }

    // Update is called once per frame
    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0)
        {
            
        }
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            if (health != null)
            {
                health.reduceHealth(damage);
            }
        }

        Instantiate(explosion, transform.position, new Quaternion());
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Muzzle") && !other.CompareTag(shooterTag))
        {
            Explode();    
        }
        
    }
}
