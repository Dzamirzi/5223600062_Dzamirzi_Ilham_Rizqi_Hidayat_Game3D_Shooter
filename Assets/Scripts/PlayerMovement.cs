using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed;
    public float lookSpeed;
    public CharacterController controller;
    public Transform cameraTransform;
    public Transform camlookAtPivot;
    public Transform cameraLookAt;
    public Animator animator;

    float camLookAtPivotX;

    // --- VARIABEL GRAVITASI BARU ---
    private Vector3 playerVelocity;
    private float gravityValue = -9.81f;
    // -----------------------------

    private void Start()
    {
        cameraTransform.SetParent(transform);
    }

    private void Update()
    {
        Move();
        Look();
    }

    void Move()
    {
        bool isGrounded = controller.isGrounded;

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f;
        }

        // Kalkulasi gerakan XZ (Kiri/Kanan/Maju/Mundur)
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(inputHorizontal, 0, inputVertical).normalized;
        movement = transform.TransformDirection(movement);

        playerVelocity.y += gravityValue * Time.deltaTime;


        controller.Move((movement * moveSpeed + playerVelocity) * Time.deltaTime);

        animator.SetFloat("Horizontal", inputHorizontal);
        animator.SetFloat("Vertical", inputVertical);
    }

    void Look()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        transform.eulerAngles += new Vector3(0, mouseX * lookSpeed * Time.deltaTime, 0);
        cameraTransform.LookAt(cameraLookAt);

        camLookAtPivotX += -mouseY * lookSpeed * Time.deltaTime;
        camLookAtPivotX = Mathf.Clamp(camLookAtPivotX, -30, 30);
        camlookAtPivot.localEulerAngles = new Vector3(camLookAtPivotX, 0, 0);
    }
}