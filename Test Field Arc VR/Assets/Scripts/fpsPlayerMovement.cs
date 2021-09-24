using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fpsPlayerMovement : MonoBehaviour
{
    public CharacterController controller;

    public float WalkingSpeed = 2f;
    public float RunningSpeed = 5f;

    public float gravity = -9.81f;

    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    bool isGrounded;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -1f;
        }

        if (!isGrounded)
            velocity.y += gravity * Time.deltaTime;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        bool run = Input.GetAxis("Run") > 0.1f;

        Vector3 move = transform.right * x + transform.forward * z;

        move.Normalize();

        if (!run)
            controller.Move(move * WalkingSpeed * Time.deltaTime);
        else
            controller.Move(move * RunningSpeed * Time.deltaTime);


        controller.Move(velocity * Time.deltaTime);
    }
}
