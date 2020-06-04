using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIControls : MonoBehaviour
{
    
    public float movementSpeed;
    public float turningSpeed;
    public float turretTurningSpeed;
    public float shootingCooldown;
    public float detectRange;
    public float stoppingRange;
    public float switchTargetRange;
    public float switchDistance;
    public float AIDelay;
    public string stringState;    // just for editor info
    public Transform turret;
    public Transform muzzle;
    public GameObject projectile;
    
    private float t;
    private float AIt;
    private int obstacleMask;
    private Rigidbody rb;
    private GameObject targetObject;
    private Vector3 target;
    
    private enum State
    {
        forward,
        left,
        right,
        back,
        stop
    };

    private State state;
    private State nextState;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        t = 0f;
        AIt = 0f;
        obstacleMask = LayerMask.GetMask("Obstacle");
        state = State.forward;
        nextState = State.forward;
    }

    
    private void FixedUpdate()
    {
        t -= Time.deltaTime;
        
        Vector3 currentRotation = rb.transform.eulerAngles;
        rb.rotation = Quaternion.Euler(0f, currentRotation.y, 0f);    // tank is allowed to rotate only around y-axis  --> prevents from rolling over
        
        // switch target randomly if player not within range
        if (Vector3.Distance(transform.position, target) < switchTargetRange)
        {
            float randomx = Random.Range(-switchDistance, switchDistance);
            float randomz = Random.Range(-switchDistance, switchDistance);
            
            target += new Vector3(randomx, 0f, randomz);
        }
        
        // find player: player should be within detecting range and loS should be clear
        if (targetObject != null)
        {
            // player is detected only within a defined range
            if (Vector3.Distance(transform.position, targetObject.transform.position) < detectRange)
            {
                // LoS check: there are no obstacles between enemy and player 
                if (!Physics.Linecast(transform.position, targetObject.transform.position, obstacleMask))
                {
                    // set target and fire
                    target = targetObject.transform.position;
                    if (t < 0)
                    {
                        GameObject proj = Instantiate(projectile, muzzle.position, muzzle.rotation);
                        proj.GetComponent<Projectile>().shooterTag = tag;
                        t = shootingCooldown;    
                    }
                    
                    // check if player is too close --> stop movement to prevent explosion damage
                    if (Vector3.Distance(target, transform.position) < stoppingRange)
                    {
                        nextState = State.stop;
                    } 
                }
            }
        }
        else
        {
            targetObject = GameObject.FindGameObjectWithTag("Player");    
        }
        
        // move towards player: in which angle player is aligned towards (this) enemy
        float angle = Vector3.SignedAngle(transform.forward, target - transform.position, Vector3.up);

        // AIt updater
        if (AIt < 0)
        {
            state = nextState;
            AIt = AIDelay;
        }
        else
        {
            AIt -= Time.deltaTime;
        }
        
        // state "machine"
        if (state == State.forward)
        {
            stringState = "forward";
            if (angle < 0)
            {
                Turning((-1f));
            }
            else if (angle > 0)
            {
                Turning(1f);
            }
            // player is in front of 180 deg view --> if so, move towards player  
            if (Math.Abs(angle) < 90)
            {
                Move(1f);
            }
        }
        else if (state == State.left)
        {
            stringState = "left";
            Turning((-1f));
            Move(1f);
        }
        else if (state == State.right)
        {
            stringState = "right";
            Turning(1f);
            Move(1f);
        }
        else if (state == State.back)
        {
            stringState = "back";
            Move(-1f);
            nextState = State.forward;
        }
        else if (state == State.stop)
        {
            stringState = "stop";
            Move(0f);
            nextState = State.forward;
        }
        
        // turret turning towards target
        Vector3 targetDirection = target - turret.position;
        targetDirection.y = 0f;
        Vector3 turningDirection = Vector3.RotateTowards(turret.forward, targetDirection, turretTurningSpeed * Time.deltaTime, 0f);
        turret.rotation = Quaternion.LookRotation(turningDirection);

    }

    private void Move(float input)
    {
        Vector3 movement = transform.forward * input * movementSpeed;
        rb.velocity = movement;
    }

    private void Turning(float input)
    {
        Vector3 turning = Vector3.up * input * turningSpeed;    
        rb.angularVelocity = turning;   
    }

    // obstacle is inside bumber collider --> decide which direction to turn
    private void OnTriggerEnter(Collider other)
    {
        // obstacle is player --> no change
        if (!other.gameObject.CompareTag("Obstacle") && !other.gameObject.CompareTag("Wall"))
        {
            return;
        }

        // obstacle is block or wall --> shoot two rays left & right (45 deg angle): turn towards the ray with longer length
        RaycastHit leftHit;
        RaycastHit rightHit;

        float leftLength = 0f;
        float rightLength = 0f;

        // there is no transform.left vector --> right vector is mirrored by multiplying with -1 --> turn left
        if (Physics.Raycast(transform.position, transform.forward + transform.right * -1, out leftHit, Mathf.Infinity,
            obstacleMask))
        {
            leftLength = leftHit.distance;
        }
        // turn right
        if (Physics.Raycast(transform.position, transform.forward + transform.right, out rightHit, Mathf.Infinity,
            obstacleMask))
        {
            rightLength = rightHit.distance;
        }

        if (leftLength > rightLength)
        {
            state = State.left;
            target = leftHit.point;
        }
        else
        {
            state = State.right;
            target = rightHit.point;
        }
    }
    
    // bumber collider is cleared of any obstacles --> forward
    private void OnTriggerExit(Collider other)
    {
        nextState = State.forward;
    }
    
    // bumber is touched by an obstacle --> stop and reverse
    private void OnCollisionEnter(Collision other)
    {
        // obstacle is player --> no change
        if (!other.gameObject.CompareTag("Obstacle") && !other.gameObject.CompareTag("Wall"))
        {
            return;
        }

        state = State.back;
    }
}
