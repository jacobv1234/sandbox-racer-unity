using System;
using System.Linq; // array tools such as Concat and Contains
using UnityEngine;
using TMPro;

public class PlayerControl : MonoBehaviour
{
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

    [SerializeField]
    private float scale = 0.8f;
    [SerializeField]
    private GameObject checkpointParticle;
    [SerializeField]
    private GameObject finishParticle;
    [SerializeField]
    private GameObject respawnParticle;

    private ParticleSystem trail;

    bool UpPressed, DownPressed, LeftPressed, RightPressed;

    private InputSystem_Actions actions;

    private Vector3 respawnCoords;
    private Quaternion respawnRotation;

    private Track track;
    private GameObject[] checkpoints;
    private int lap;
    private int laps = 5;

    private TMP_Text lapCounter;
    private GameObject finishText;
    

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

    void Decelerate()
    {
        if (!grounded)
        {
            return;
        }
        if (speed > -topSpeed)
        {
            speed -= acceleration;
        }
        else
        {
            speed = -topSpeed;
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

    void Respawn()
    {
        transform.position = respawnCoords;
        transform.rotation = respawnRotation;
        speed = 0;
        rotation = 0;
        Instantiate(respawnParticle, respawnCoords, respawnRotation);
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

        if ((!UpPressed && !DownPressed) || !grounded) // airborne or not controlling
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

        if (!LeftPressed && !RightPressed)
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        actions = new InputSystem_Actions();
        //actions.Enable();

        respawnCoords = transform.position;
        respawnRotation = transform.rotation;
    }

    private void Start()
    {
        transform.localScale = new Vector3(scale, scale, scale);
        trail = GetComponent<ParticleSystem>();

        GameObject trackObj = GameObject.Find("TileFloor");
        if (trackObj != null)
        {
            track = trackObj.GetComponent<Track>();
        }
        lap = 1;
        laps = track.getLapCount();
        checkpoints = new GameObject[0];
        lapCounter = GameObject.Find("LapCounter").GetComponent<TMP_Text>();
        finishText = GameObject.Find("FinishText");
    }

    // Update is called once per frame
    void Update()
    {
        // controls
        UpPressed = actions.Player.Accelerate.IsPressed();
        DownPressed = actions.Player.Brake.IsPressed();
        LeftPressed = actions.Player.Left.IsPressed();
        RightPressed = actions.Player.Right.IsPressed();

        if (UpPressed) {Accelerate();}
        if (DownPressed) {Decelerate();}
        if (LeftPressed) {SteerLeft();}
        if (RightPressed) {SteerRight();}

        if (actions.Player.Respawn.WasPressedThisFrame()) {Respawn();}

        // physics
        Move();
        Turn();
        CheckAirborne();
        DecayValues();
        UpdateTrail();
    }

    private void startControls()
    {
        actions.Enable();
        updateLapCounter();
    }

    private void updateLapCounter()
    {
        lapCounter.text = "Lap: " + lap + " / " + laps;
    }


    private void OnDestroy()
    {
        if (actions != null)
        {
            actions.Disable(); // memory cleanup
        }
        lapCounter.text = ""; // hide lap counter
    }

    // stop momentum when hit wall
    private void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount > 8)
        {
            speed = 0;
        }
    }

    // hit checkpoint / finish line
    private void OnTriggerEnter(Collider other)
    {
        if (track == null)
        {
            return;
        }

        // update respawn location
        respawnCoords = other.transform.position + Vector3.up;
        respawnRotation = other.transform.rotation;

        // detect trigger type
        if (other.name.Contains("checkpoint"))
        {
            // checkpoint
            // check it's a new checkpoint
            if (!checkpoints.Contains(other.gameObject))
            {
                // create particle
                Instantiate(checkpointParticle, respawnCoords, respawnRotation);
                // register checkpoint
                checkpoints = checkpoints.Concat(new GameObject[] { other.gameObject }).ToArray();
            }
        }
        else
        {
            // finish line
            // check lap is valid
            if (track.getCheckpointCount() == checkpoints.Length)
            {
                // create particle
                Instantiate(finishParticle, respawnCoords, respawnRotation);

                // reset lap
                checkpoints = new GameObject[0];
                lap += 1;
                if (lap > laps)
                {
                    actions.Disable();
                    finishText.SendMessage("doAnimation");
                }
                else
                {
                    updateLapCounter();
                }
            }
        }
    }
}
