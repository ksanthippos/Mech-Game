using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    
    // HUD 
    public float maxHealth = 100;
    public float maxPower = 100;
    public float maxHeat = 100;
    public int maxAmmo = 10;
    public bool shieldsOn;

    public GameObject explosion;
    public  Color damageColor = Color.red;
    public float damageFlashTime;
    
    private float currentHealth;
    private float currentPower;
    private float currentHeat;
    private float currentAmmo;
    private float t;
    private bool dead = false;
    private Color originalColor;
    private Color originalEmissionColor;
    private MeshRenderer[] meshRenderers;
   
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        
        currentHealth = maxHealth;
        currentHeat = 0;
        currentPower = maxPower;
        currentAmmo = maxAmmo;
        shieldsOn = false;

        meshRenderers = GetComponentsInChildren<MeshRenderer>();
        originalColor = meshRenderers[0].material.color;    // all children same color
        audioSource = GetComponent<AudioSource>();
        
        if (gameObject.CompareTag("Player"))
        {
            GameController.instance.SetHealth(currentHealth, maxHealth);
        }
    }

    // Update is called once per frame
    public void reduceHealth(float damage)
    {
        StartCoroutine(damageFlash());
        currentHealth -= damage;
        audioSource.Play();

        if (gameObject.CompareTag("Player"))
        {
            GameController.instance.SetHealth(currentHealth, maxHealth);
        }
        
        if (currentHealth <= 0 && !dead)
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

    private IEnumerator damageFlash()
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
}
