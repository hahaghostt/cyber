using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using CyberMovementSystem;

public class CharacterController2 : MonoBehaviour
{
    public float movementSpeed = 5f;
    public float rotationSpeed = 2f;
    public float sprintSpeed = 8f;

    private Rigidbody rb;
    private Camera playerCamera;

    private float cameraRotationX = 0f;
    private float cameraRotationY = 0f;

    public float jumpForce = 10f;
    public float gravityModifier;

    public bool isOnGround = true;

    private float maxCastDistance = 5f; // Adjust the distance based on your scene
    private LayerMask obstacleLayer; // Specify the layer for obstacles

    static public bool dialogue = false; 

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        playerCamera = Camera.main;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Physics.gravity *= gravityModifier;

        obstacleLayer = LayerMask.GetMask("obstacles"); 
    }

    private void Update()
    {
        HandleInput();
        // MoveCharacter();
        RotateCamera();

        if (!CharacterController2.dialogue)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            RotateCamera();

            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0, 0);

            
            
        }

        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            cameraRotationX = Mathf.Clamp(cameraRotationX, 0, 0f);
            cameraRotationY = Mathf.Clamp(cameraRotationY, 0, 0f);
            rb.freezeRotation = true;
           

        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isOnGround)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isOnGround = false;
        }
    }

    private void MoveCharacter()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
        Vector3 moveVelocity = transform.TransformDirection(moveDirection) * (movementSpeed);

        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }

    private void RotateCamera()
    {
        float mouseX = Input.GetAxis("Mouse X") * rotationSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * rotationSpeed;

        transform.Rotate(Vector3.up, mouseX);
        transform.Rotate(Vector3.up, mouseY);

        cameraRotationX -= mouseY;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90f, 90f);

        cameraRotationY -= mouseX;
        // cameraRotationY = Mathf.Clamp(cameraRotation, -90f, 90f);*/

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationY, 30f, 30f);


        if (Physics.SphereCast(playerCamera.transform.position, 10f, playerCamera.transform.forward, out RaycastHit hit, maxCastDistance, obstacleLayer))
        {
            
            LimitCameraRotation(hit.distance);
        }
        else
        {
            // If no obstacle is hit, set the normal camera rotation
            playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
        }
    }

    private void FixedUpdate()
    {
        if(!dialogue)
        {
            MoveCharacter(); 
        }
    }

    private void LimitCameraRotation(float distanceToObstacle)
    {
        // Adjust the camera's local rotation based on the distance to the obstacle
        float maxRotationX = Mathf.Atan(distanceToObstacle / 10f) * Mathf.Rad2Deg;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -maxRotationX, maxRotationX);

        playerCamera.transform.localRotation = Quaternion.Euler(cameraRotationX, 0f, 0f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isOnGround = true;
    }
}