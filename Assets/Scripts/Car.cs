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

    [Header("Steering")]

    public float maxTurnSpeed;
    public float turnAcceleration;
    public float turnDrag;

    [Header("Sensors")]

    public Transform groundCheckOrigin;
    public float groundCheckDistance = 1;

    [Header("Debug")]

    public Transform debugText;

    float speed;
    float turnSpeed;

    float angle;

    Rigidbody rigidbody;

    bool isGrounded;

    TextMesh speedTextC;

    // Control flags

    bool accelerate;
    bool reverse;
    bool turnLeft;
    bool turnRight;
    bool handBrake;



    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        speedTextC = debugText.GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) { accelerate = true; }
        else if (Input.GetKey(KeyCode.S)) { reverse = true; }

        if (Input.GetKey(KeyCode.A)) { turnLeft = true; }
        else if(Input.GetKey(KeyCode.D)) { turnRight = true; }

        handBrake = Input.GetKey(KeyCode.Space);

        speedTextC.text = String.Format("{0:000}", rigidbody.velocity.magnitude);

        
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

        if(!accelerate || !isGrounded)
        {
            if(speedLocal.z > 0)
            {
                speedLocal -= new Vector3(0, 0, frontDrag * Time.deltaTime);
                if (speedLocal.z < 0) { speedLocal = new Vector3(speedLocal.x, speedLocal.y, 0); }
            }

        }

        // Apply side drag

        if(isGrounded && !handBrake)
        {
            if (speedLocal.x > 0)
            {
                speedLocal -= new Vector3(sidesDrag * Time.deltaTime, 0, 0);
                if (speedLocal.x < 0) { speedLocal = new Vector3(0, speedLocal.y, speedLocal.z); }
            }
            else if (speedLocal.x < 0)
            {
                speedLocal += new Vector3(sidesDrag * Time.deltaTime, 0, 0);
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


        accelerate = false;
        reverse = false;
        turnLeft = false;
        turnRight = false;
    }


}
