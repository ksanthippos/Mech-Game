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
    
    private Rigidbody rb;
    private Camera mainCamera;
    private GameController gameController;
    private float maxRayDistance = 100f;
    private float t;
    private int floorMask;
    
    
    // Start is called before the first frame update
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
        gameController = GameController.instance;
        
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
                }
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
        if (inputHorizontal != 0)
        {
            Vector3 turning = Vector3.up * inputHorizontal * turningSpeed;    
            rb.angularVelocity = turning;   
        }

        // moving forw/backw
        if (inputVertical != 0)
        {
            Vector3 movement = transform.forward * inputVertical * movementSpeed;
            rb.velocity = movement;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);    // ray pointing towards mouse cursor
        RaycastHit hit;    // point where ray hits

        if (Physics.Raycast(ray, out hit, maxRayDistance, floorMask))
        {
            Vector3 targetDirection = hit.point - turret.position;
            targetDirection.y = 0f;
            Vector3 turningDirection = Vector3.RotateTowards(turret.forward, targetDirection, turretTurningSpeed * Time.deltaTime, 0f);
            turret.rotation = Quaternion.LookRotation(turningDirection);
        }
    }

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
