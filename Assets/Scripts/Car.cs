using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Car : MonoBehaviour
{
    [Header("Movement")]

    public float maxSpeed;
    public float acceleration;

    public float frontDrag;
    public float sidesDrag;

    public float handBrakeFrontDrag;
    public float handBrakeSidesDrag;

    [Header("Steering")]

    public float maxTurnSpeed;
    public float turnAcceleration;
    public float turnDrag;

    [Header("Sensors")]

    public Transform groundCheckOrigin;
    public float groundCheckDistance = 1;

    [Header("Sounds")]

    public float windVolumeMin;
    public float windVolumeMax;
    public float windPitchMin;
    public float windPitchMax;
    public Transform windSound;

    public float engineVolumeMin;
    public float engineVolumeMax;
    public float enginePitchMin;
    public float enginePitchMax;
    public Transform engineSound;

    [Header("Debug")]

    public Transform debugText1;
    public Transform debugText2;

    float speed;
    float turnSpeed;

    float angle;

    Rigidbody rigidbody;

    bool isGrounded;

    TextMesh speedTextC;
    TextMesh timeTextC;

    // Control flags

    bool accelerate;
    bool reverse;
    bool turnLeft;
    bool turnRight;
    bool handBrake;

    // Sound

    AudioSource engineSoundC;
    AudioSource windSoundC;

    float debugTimer;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        speedTextC = debugText1.GetComponent<TextMesh>();
        timeTextC = debugText2.GetComponent<TextMesh>();
        engineSoundC = engineSound.GetComponent<AudioSource>();
        windSoundC = windSound.GetComponent<AudioSource>();

        debugTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) { accelerate = true; }
        else if (Input.GetKey(KeyCode.S)) { reverse = true; }

        if (Input.GetKey(KeyCode.A)) { turnLeft = true; }
        else if(Input.GetKey(KeyCode.D)) { turnRight = true; }

        handBrake = Input.GetKey(KeyCode.Space);

        debugTimer += Time.deltaTime;
        speedTextC.text = String.Format("{0:000}", rigidbody.velocity.magnitude);
        timeTextC.text = String.Format("{0:0}:{1:00}", (int)(debugTimer / 60), (int)(debugTimer) % 60);
        
    }

    void FixedUpdate()
    {

        // Update grounded state

        RaycastHit hit;

        if (Physics.Raycast(groundCheckOrigin.position, - groundCheckOrigin.up, out hit, groundCheckDistance)) { isGrounded = true;  }
        else { isGrounded = false; }

        Vector3 speed = rigidbody.velocity;

        // Apply acceleration and reverse

        if(isGrounded)
        {
            if (accelerate) { speed += transform.forward * acceleration * Time.deltaTime; }
            else if (reverse) { speed -= transform.forward * acceleration * Time.deltaTime; }

            if (speed.magnitude > maxSpeed) { speed = speed.normalized * maxSpeed; }
        }

        // Get speed in local system

        Vector3 speedLocal = transform.InverseTransformVector(speed);


        // Apply front drag

        if(isGrounded  && (!accelerate && !reverse || handBrake))
        {
            if(speedLocal.z > 0)
            {
                speedLocal -= new Vector3(0, 0, (handBrake ? handBrakeFrontDrag : frontDrag) * Time.deltaTime);
                if (speedLocal.z < 0) { speedLocal = new Vector3(speedLocal.x, speedLocal.y, 0); }
            }

        }

        // Apply side drag

        if(isGrounded)
        {
            float drag = (handBrake ? handBrakeSidesDrag : sidesDrag);

            if (speedLocal.x > 0)
            {
                speedLocal -= new Vector3(drag * Time.deltaTime, 0, 0);
                if (speedLocal.x < 0) { speedLocal = new Vector3(0, speedLocal.y, speedLocal.z); }
            }
            else if (speedLocal.x < 0)
            {
                speedLocal += new Vector3(drag * Time.deltaTime, 0, 0);
                if (speedLocal.x > 0) { speedLocal = new Vector3(0, speedLocal.y, speedLocal.z); }
            }
        }

        // Apply local speed to world

        speed = transform.TransformVector(speedLocal);

        // Get angular speed

        Vector3 angularSpeed = rigidbody.angularVelocity;

        // Apply steering

        if(isGrounded)
        {
            if (turnLeft)
            {
                angularSpeed -= new Vector3(0, turnAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);

            }
            else if (turnRight)
            {
                angularSpeed += new Vector3(0, turnAcceleration * Mathf.Deg2Rad * Time.deltaTime, 0);
            }
        }

        // Apply steer drag

        if (!turnLeft && !turnRight || !isGrounded)
        {
            if(angularSpeed.y > 0)
            {
                angularSpeed -= new Vector3(0, turnDrag * Mathf.Deg2Rad * Time.deltaTime, 0);
                if(angularSpeed.y < 0) { angularSpeed = new Vector3(angularSpeed.x, 0, angularSpeed.z); }
            }
            else if (angularSpeed.y < 0)
            {
                angularSpeed += new Vector3(0, turnDrag * Mathf.Deg2Rad * Time.deltaTime, 0);
                if (angularSpeed.y > 0) { angularSpeed = new Vector3(angularSpeed.x, 0, angularSpeed.z); }
            }

        }

        if (angularSpeed.magnitude > maxTurnSpeed * Mathf.Deg2Rad) { angularSpeed = angularSpeed.normalized * maxTurnSpeed * Mathf.Deg2Rad; }


        rigidbody.velocity = speed;
        rigidbody.angularVelocity = angularSpeed;

        // Update sounds


        float lerpFactor = speed.magnitude / maxSpeed;
        engineSoundC.pitch = enginePitchMin + (enginePitchMax - enginePitchMin) * lerpFactor;
        engineSoundC.volume = engineVolumeMin + (engineVolumeMax - engineVolumeMin) * lerpFactor;
        windSoundC.pitch = windPitchMin + (windPitchMax - windPitchMin) * lerpFactor;
        windSoundC.volume= windVolumeMin + (windVolumeMax - windVolumeMin) * lerpFactor;


        accelerate = false;
        reverse = false;
        turnLeft = false;
        turnRight = false;
    }


}
