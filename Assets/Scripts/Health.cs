using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    
    public float maxHealth;
    public float maxPower;
    public float maxHeat;
    public float damageFlashTime;
    public float heatCoolingRate;
    public bool shieldsOn;
    public GameObject explosion;
    public  Color damageColor = Color.red;
    
    private float t;
    private float currentHealth;
    private float currentPower;
    private float currentHeat;
    private bool overHeat;
    private bool dead;
    private Color originalColor;
    private Color originalEmissionColor;
    private MeshRenderer[] meshRenderers;
   
    private AudioSource audioSource;
    
    
    void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        currentHeat = 0f;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalColor = meshRenderers[0].material.color;    // all children same color
        audioSource = GetComponent<AudioSource>();
        
        if (gameObject.CompareTag("Player"))
        {
            GameController.instance.SetHealth(currentHealth, maxHealth);
            GameController.instance.SetPower(currentPower, maxPower);
            GameController.instance.SetHeat(currentHeat, maxHeat);
            GameController.instance.SetShields(false);
        }
    }

    // called whenever a gameobject is hit
    public void ReduceHealth(float damage, float heatDamage, string tag)
    {
        StartCoroutine(DamageFlash());

        if (tag.Equals("Proj_Bullet"))
        {
            // no heat damage from bullets
            if (shieldsOn)
            {
                if (currentPower - damage > 0)
                {
                    currentPower -= damage;
                }
                else
                {
                    currentPower = 0f;
                    shieldsOn = false;
                }
            }
            else
            {
                currentHealth -= damage;
            }
        }
        else if (tag.Equals("Proj_Missile"))
        {
            HeatCheck(heatDamage, tag);
            if (shieldsOn)
            {
                if (currentPower - damage > 0)
                {
                    currentPower -= damage / 6;    // missiles have only minor impact on shields
                }
                else
                {
                    currentPower = 0f;
                    shieldsOn = false;
                }
            }
            else
            {
                currentHealth -= damage;
            }
        }
        else if (tag.Equals("Proj_Beam"))
        {
            HeatCheck(heatDamage, tag);
            if (shieldsOn)
            {
                if (currentPower - damage > 0)
                {
                    currentPower -= damage;    // full impact on shields
                }
                else
                {
                    currentPower = 0f;
                    shieldsOn = false;
                }
            }
            else
            {
                currentHealth -= damage;
            }
        }
        
        audioSource.Play();

        if (gameObject.CompareTag("Player"))
        {
            GameController.instance.SetHealth(currentHealth, maxHealth);
            GameController.instance.SetPower(currentPower, maxPower);
            GameController.instance.SetHeat(currentHeat, maxHeat);

            if (currentPower <= 0)
                GameController.instance.SetShields(false);

        }
        
        DeathCheck();    // death or nay?

    }

    // damage light effect
    private IEnumerator DamageFlash()
    {
        t = damageFlashTime;
        while (t > 0)
        {
            t -= Time.deltaTime;
            foreach (MeshRenderer r in meshRenderers)
            {
                r.material.color = damageColor;
                r.material.SetColor("_EmissionColor", damageColor);
                yield return null;
            }
        }
        
        // reset colors
        foreach (MeshRenderer r in meshRenderers)
        {
            r.material.color = originalColor;
            r.material.SetColor("_EmissionColor", originalEmissionColor);
        }
    }

    // getter & setters for power and heat
    public float GetCurrentPower()
    {
        return currentPower;
    }

    public void ReducePower(float value)
    {
        currentPower -= value;
    }

    public float GetCurrentHeat()
    {
        return currentHeat;
    }

    public void AddHeat(float value)
    {
        currentHeat += value;
        if (currentHeat >= maxHeat)
            overHeat = true;
    }
    
    // checking if gameobject has HP 0% or heat 100% --> DEATH!
    public void DeathCheck()
    {
        if ((currentHealth <= 0 || overHeat) && !dead)
        {
            dead = true;
            if (gameObject.CompareTag("Enemy") || gameObject.CompareTag("Enemy_Tank") || gameObject.CompareTag("Enemy_Turret") || gameObject.CompareTag("Enemy_MissileTank") 
                || gameObject.CompareTag("Enemy_SAM") || gameObject.CompareTag("Enemy_BeamAPC") || gameObject.CompareTag("Enemy_CannonAPC"))
            {
                GameController.instance.EnemyDestroyed();   
            }
            
            Instantiate(explosion, transform.position, new Quaternion());
            Destroy(gameObject);
        }
    }
    
    // beam adds most heat damage, shields reduce some
    private void HeatCheck(float heatDamage, string tag)
    {
        if (tag.Equals("Proj_Missile"))
        {
            if (shieldsOn && currentHeat + heatDamage < 100)    
                AddHeat(heatDamage / 4);
            else
                AddHeat(heatDamage);
        }
        else if (tag.Equals("Proj_Beam"))
        {
            if (shieldsOn && currentHeat + heatDamage < 100)
                AddHeat(heatDamage / 2);
            else
                AddHeat(heatDamage);
        }
    }
    
    
}
