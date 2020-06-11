using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    public float speed;
    public float maxSpeed;
    public float acceleration;
    public float time;
    public float radius;
    public float damage;
    public float heatDamage;
    public float shootingCooldown;
    public float lifeTime;
    public string shooterTag;
    public GameObject explosion;

    private float t;
    private float maxRayDistance = 100f;
    private int floorMask;
    private Rigidbody rb;
    private AudioSource audioSource;
    private Camera mainCamera;
    private PlayerControls playerControls;

    
    void Start()
    {
        t = time;
        rb = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
        
        if (tag.Equals("Proj_Bullet") || tag.Equals("Proj_Beam"))   // bullet and beam travel at constant speed 
        {
            rb.velocity = transform.forward * speed;
        }
    }
    
    void Update()
    {
        t -= Time.deltaTime;
        if (t < 0)
        {
            t = time;
        }
        
        if (tag.Equals("Proj_Missile"))    // missile is accelerated
        {
            if (speed < maxSpeed)
                speed += acceleration * Time.deltaTime * 100;
            
            rb.velocity = transform.forward * speed;
        }
        
        // projectiles are destroyed afterwards
        lifeTime -= Time.deltaTime;
        
        if (lifeTime <= 0) 
            Destroy(gameObject);
        
    }

    private void Explode()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            Health health = colliders[i].GetComponent<Health>();
            if (health != null)
            {
                health.ReduceHealth(damage, heatDamage, tag);    // --> tag as a parameter
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
