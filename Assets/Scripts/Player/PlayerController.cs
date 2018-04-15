using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {

    public bool movementEnabled = true;

    Transform cam;
    CharacterController controller;
    float verticalSpeed = 0f;
    public float moveSpeed = 2f;
    public float turnSpeed = 90f;
    public float gravity = 9.81f;
    public float jumpSpeed = 5f;
    public float sprintSpeed = 3.5f;

    Vector3 move;
    Vector3 mover;

    void Awake ()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main.transform;
        movementEnabled = true;
    }

    private void Update()
    {
        CalculateMovement();

        float turnAmount = Mathf.Atan2(move.x, move.z);
        float turnSpeed = Mathf.Lerp(180, 360, move.z);

        transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        controller.Move(mover * Time.deltaTime);
    }

    void CalculateMovement()
    {
        Vector3 forward = Vector3.Scale(cam.forward, new Vector3(1, 0, 1).normalized);

        float horIn = Input.GetAxis("Horizontal");
        float verIn = Input.GetAxis("Vertical");

        if (!movementEnabled)
        {
            horIn = 0f;
            verIn = 0f;
        }

        move = verIn * forward + horIn * cam.right;
        mover = move;

        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);

        if (controller.isGrounded)
        {
            verticalSpeed = 0;
            if (Input.GetButtonDown("Jump"))
            {
                verticalSpeed = jumpSpeed;
            }
        }
        mover.Normalize();
        mover *= moveSpeed;
        verticalSpeed -= gravity * Time.deltaTime;
        mover.y = verticalSpeed;
    }    

    public void EnableMovement()
    {
        movementEnabled = true;
    }
    public void DisableMovement()
    {
        movementEnabled = false;
    }
}
