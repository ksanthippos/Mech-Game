using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    
    public enum Weapon
    {
        Autocannon, Missiles, Beam
    }
    
    public float movementSpeed;
    public float turningSpeed;
    public float turretTurningSpeed;
    public float shootingCooldown;
    public int weaponIndex;
    public int maxAmmo;
    public int currentAmmo;

    public Transform turret;
    public Transform muzzle;
    public Weapon weapon;
    public List<GameObject> projectiles;
    public GameObject bulletFlash;
    public GameObject beamFlash;
    
    private Rigidbody rb;
    private Camera mainCamera;
    private GameController gameController;
    private Animator animator;
    private float maxRayDistance = 100f;
    private float t;
    private int floorMask;
    
    
    void Start()
    {
        t = 0f;
        weaponIndex = 0;
        currentAmmo = maxAmmo;
        SetWeapon();
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        floorMask = LayerMask.GetMask("Floor");
        shootingCooldown = projectiles[weaponIndex].GetComponent<Projectile>().shootingCooldown;    // default first from the list -> autocannon
        bulletFlash = GameObject.FindWithTag("Bullet_Flash");
        bulletFlash.SetActive(false);
        beamFlash = GameObject.FindWithTag("Beam_Flash");
        beamFlash.SetActive(false);
        gameController = GameController.instance;
        animator = GameObject.FindWithTag("Torso_Low").GetComponent<Animator>();    // animator resides in the lower torso, which is a child object
    }
    
    private void Update()
    {
        // set current weapon
        shootingCooldown = projectiles[weaponIndex].GetComponent<Projectile>().shootingCooldown;
        SetWeapon();

        if (t <= 0)    // cooling time ok
        {
            if (Input.GetButton("Fire1"))
            {
                if (gameController.CheckOkToShoot(weapon))    
                {
                    GameObject proj1 = Instantiate(projectiles[weaponIndex], muzzle.position, muzzle.rotation);
                    proj1.GetComponent<Projectile>().shooterTag = tag;
                    t = shootingCooldown;
                    
                    if (weapon == Weapon.Autocannon)
                        bulletFlash.SetActive(true);
                    else if (weapon == Weapon.Beam)
                        beamFlash.SetActive(true);
                }
            }
            else
            {
                bulletFlash.SetActive(false);    
                beamFlash.SetActive(false);
            }
        }
        else 
        {
            t -= Time.deltaTime;
        }
    }
    
    void FixedUpdate() // 
    {
        Vector3 currentRotation = rb.transform.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);    // tank is allowed to rotate only around y-axis  --> prevents from rolling over
        
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");

        // turning left and right
        if (inputHorizontal != 0 && inputVertical != 0)
        {
            Vector3 turning = Vector3.up * inputHorizontal * turningSpeed;    
            rb.angularVelocity = turning; 
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        
        // forward: walk & run
        if (inputVertical > 0)
        {
            Vector3 movement = transform.forward * inputVertical * movementSpeed;
            rb.velocity = movement;
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);

            if (Input.GetKey(KeyCode.LeftShift))    // run
            {
                Vector3 movement2 = transform.forward * inputVertical * movementSpeed * 2;
                rb.velocity = movement2;
                animator.SetBool("Run", true);
                animator.SetBool("Walk", false);
            }
        }
        
        // reverse is 50% slower
        if (inputVertical < 0)
        {
            Vector3 movement = transform.forward * inputVertical * (movementSpeed * 0.7f);
            rb.velocity = movement;
            animator.SetBool("Walk", true);
            animator.SetBool("Run", false);
        }
        
        // not moving
        if (inputHorizontal == 0 && inputVertical == 0)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        } 
        
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);    // ray pointing towards mouse cursor
        RaycastHit hit;    // point where ray hits

        // turret aiming
        if (Physics.Raycast(ray, out hit, maxRayDistance, floorMask))
        {
            Vector3 targetDirection = hit.point - turret.position;
            targetDirection.y = 0f;
            
            Vector3 turningDirection = Vector3.RotateTowards(turret.forward, targetDirection, turretTurningSpeed * Time.deltaTime, 0f);
            turret.rotation = Quaternion.LookRotation(turningDirection);
        }
    }

    // weapon enum setup
    private void SetWeapon()
    {
        if (weaponIndex == 0)
        {
            weapon = Weapon.Autocannon;
        }
        else if (weaponIndex == 1)
        {
            weapon = Weapon.Missiles;
        }
        else if (weaponIndex == 2)
        {
            weapon = Weapon.Beam;
        }
    }

}
