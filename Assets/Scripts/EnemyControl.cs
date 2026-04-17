using Unity.Collections;
using UnityEngine;
using System.Collections.Generic;

public class EnemyControl : MonoBehaviour
{
    [SerializeField]
    private float scale = 0.5f;

    private List<List<int>> path;

    private bool AIEnabled = false;
    

    // AI controls
    [SerializeField]
    // how close the angle of the car is to facing the center of the next tile when it stops turning
    private float turnPrecision = 5.0f;

    [SerializeField]
    // how far away the angle of the car is from facing the center of the next tile when it stops accelerating
    private float turnOnSpotCutoff = 60.0f;

    [SerializeField]
    // how close the car is to the center of the next tile before it changes target (in tiles)
    private float targetRadius = 0.8f;


    // Car physics
    [SerializeField]
    private float turnSpeed = 200.0f;
    [SerializeField]
    private float rotationAccel = 10.0f;
    [SerializeField]
    private float rotationFriction = 50.0f;
    [SerializeField]
    private float topSpeed = 20.0f;
    [SerializeField]
    private float acceleration = 0.3f;
    [SerializeField]
    private float groundFriction = 1.0f;
    [SerializeField]
    private float airFriction = 0.3f;

    private float objectHeight = 1.5f;

    [SerializeField]
    private float speed = 0.0f;
    [SerializeField]
    private float rotation = 0.0f;
    [SerializeField]
    private bool grounded = false;

    private bool isAccelerating = false;
    private bool isSteering = false;

    // SerializeField to assist in debugging
    // coordinates on the same scale as path
    [SerializeField, ReadOnly]
    private float relx;
    [SerializeField, ReadOnly]
    private float rely;
    [SerializeField, ReadOnly]
    private float angle;

    // index into path to currently aim toward
    [SerializeField, ReadOnly]
    private int target;

    [SerializeField, ReadOnly]
    private float tx;
    [SerializeField, ReadOnly]
    private float ty;
    [SerializeField, ReadOnly]
    private float distance;
    [SerializeField, ReadOnly]
    private float tAngle;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AIEnabled)
        {
            return;
        }

        relx = (transform.position.x + 40.0f) / 10.0f;
        rely = (transform.position.z + 40.0f) / 10.0f;
        angle = transform.rotation.eulerAngles.y;

        // target coordinate
        tx = path[target][0];
        ty = path[target][1];

        // offset
        float xo = tx - relx;
        float yo = ty - rely;

        // distance
        distance = Mathf.Sqrt(xo * xo + yo * yo);

        // target angle
        tAngle = Mathf.Rad2Deg * Mathf.Atan2(xo, yo);
        // adjust mapping to match transform.rotation
        if (tAngle < 0)
        {
            tAngle += 360;
        }

        // work out if turning left or right is faster
        float leftAngle;
        float rightAngle;

        leftAngle = angle - tAngle;
        if (leftAngle < 0)
        {
            leftAngle += 360;
        }
        rightAngle = 360 - leftAngle;

        float minAngle = Mathf.Min(leftAngle, rightAngle);


        // control car
        if (minAngle < turnOnSpotCutoff)
        {
            Accelerate();
            isAccelerating = true;
        }
        else
        {
            isAccelerating = false;
        }

        if (minAngle > turnPrecision)
        {
            if (leftAngle > rightAngle)
            {
                SteerRight();
                isSteering = true;
            }
            else
            {
                SteerLeft();
                isSteering = true;
            }
        }
        else
        {
            isSteering = false;
        }

        // update target
        if (distance < targetRadius)
        {
            target += 1;
            // loop round at the end of the lap
            if (target >= path.Count)
            {
                target = 0;
            }
        }

        // do car physics
        Move();
        Turn();
        CheckAirborne();
        DecayValues();
    }


    // activate AI (and get path)
    // path is got here (instead of Start()) to ensure the entire path is loaded
    void startDriving()
    {
        AIEnabled = true;
        path = GameObject.Find("TileFloor").GetComponent<Track>().getPath();
    }



    // Car physics functions (identical to PlayerControl.cs)
    void Accelerate()
    {
        if (!grounded)
        {
            return;
        }
        if (speed < topSpeed)
        {
            speed += acceleration;
        }
        else
        {
            speed = topSpeed;
        }
    }

    void SteerRight()
    {
        if (!grounded)
        {
            return;
        }
        if (rotation < 0)
        {
            rotation = 0;
        }
        if (rotation < turnSpeed)
        {
            rotation += rotationAccel;
        }
        else
        {
            rotation = turnSpeed;
        }
    }

    void SteerLeft()
    {
        if (!grounded)
        {
            return;
        }
        if (rotation > 0)
        {
            rotation = 0;
        }
        if (rotation > -turnSpeed)
        {
            rotation -= rotationAccel;
        }
        else
        {
            rotation = -turnSpeed;
        }
    }

    void Move()
    {
        Vector3 moveVector = new Vector3(0, 0, speed);

        transform.Translate(moveVector * Time.deltaTime);
    }

    void Turn()
    {
        Vector3 rotationVector = new Vector3(0, rotation, 0);
        transform.Rotate(rotationVector * Time.deltaTime);
    }

    void CheckAirborne()
    {
        Vector3 offset = transform.rotation * Vector3.up;
        grounded = Physics.Raycast(transform.position + offset, transform.rotation * Vector3.down, objectHeight);
    }

    void DecayValues()
    {
        float friction;
        if (grounded)
        {
            friction = groundFriction;
        }
        else
        {
            friction = airFriction;
            rotation = 0.0f; // unable to steer in air
        }

        if (!isAccelerating || !grounded) // airborne or not controlling
        {
            if (speed < 0.0f)
            {
                speed += topSpeed * friction * Time.deltaTime;
                if (speed > 0.0f)
                {
                    speed = 0.0f;
                }
            }
            else
            {
                speed -= topSpeed * friction * Time.deltaTime;
                if (speed < 0.0f)
                {
                    speed = 0.0f;
                }
            }
        }

        if (!isSteering)
        {
            if (rotation < 0.0f)
            {
                rotation += turnSpeed * rotationFriction * Time.deltaTime;
                if (rotation > 0.0f)
                {
                    rotation = 0.0f;
                }
            }
            else
            {
                rotation -= turnSpeed * rotationFriction * Time.deltaTime;
                if (rotation < 0.0f)
                {
                    rotation = 0.0f;
                }
            }
        }
    }

    // stop momentum when hit wall
    private void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount > 8)
        {
            speed = 0;
        }
    }

    /*
     * Not currently in use
     * May activate at some point to give a particle trail to the opponent
    void UpdateTrail()
    {
        if (!trail.isPlaying && speed > 0 && grounded)
        {
            trail.Play();
        }
        if (trail.isPlaying && (speed == 0 || !grounded))
        {
            trail.Stop();
        }
    }
    */
}
