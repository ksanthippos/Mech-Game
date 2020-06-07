using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    
    public float maxHealth = 100;
    public float maxPower = 100;
    public float maxHeat = 100;
    public float damageFlashTime;
    public float heatCoolingRate;
    public bool shieldsOn;
    public GameObject explosion;
    public  Color damageColor = Color.red;
    
    private float t;
    private float currentHealth;
    private float currentPower;
    private float currentHeat;
    private bool dead = false;
    private Color originalColor;
    private Color originalEmissionColor;
    private MeshRenderer[] meshRenderers;
   
    private AudioSource audioSource;

    // Start is called before the first frame update
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

    public void ReduceHealth(float damage, float heatDamage, string tag)
    {
        StartCoroutine(DamageFlash());

        if (tag.Equals("Proj_Bullet"))
        {
            // no heat check from bullets
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
        
        // 0 HP or 100% heat --> player dies
        if ((currentHealth <= 0 || currentHeat >= maxHeat) && !dead)
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
    }

    // checking for heat damage: different impact when shields on/off
    private void HeatCheck(float heatDamage, string tag)
    {
        if (tag.Equals("Proj_Missile"))
        {
            if (shieldsOn && currentHeat + heatDamage < 100)
                currentHeat += heatDamage / 4;
            else
                currentHeat += heatDamage;
        }
        else if (tag.Equals("Proj_Beam"))
        {
            if (shieldsOn && currentHeat + heatDamage < 100)
                currentHeat += heatDamage / 2;
            else
                currentHeat += heatDamage;
        }
    }
}
