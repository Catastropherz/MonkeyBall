using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class NewBehaviourScript : MonoBehaviour
{
    public Rigidbody sphere;
    public GameObject cameraX;
    public GameObject cameraY;
    public GameObject cameraZ;
    public GameObject cameraObject;

    [SerializeField] private InputActionReference moveAction;

    private float moveSpeed = 280.0f;
    private float horizontalInput;
    private float verticalInput;
    private Vector3 moveDirection;

    private float maxVerticalAngle = 20.0f;
    private float maxHorizontalAngle = 15.0f;

    private Vector3 respawnPosition;

    void Start()
    { 
        // Save respawn position
        respawnPosition = transform.position;
    }
    
    // Movement function
    void MoveSphere()
    {
        // set input values
        //horizontalInput = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        //verticalInput = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        horizontalInput = inputVector.x * moveSpeed * Time.deltaTime;
        verticalInput = inputVector.y * moveSpeed * Time.deltaTime;
        moveDirection = (cameraX.transform.forward * verticalInput) + (cameraY.transform.right * horizontalInput);

        // add force to sphere
        sphere.AddForce(moveDirection);

    }

    // Camera function based on input
    void MoveCamera()
    {
        // set input values
        //float horizontalTilt = Input.GetAxis("Horizontal") * maxHorizontalAngle;
        //float verticalTilt = Input.GetAxis("Vertical") * maxVerticalAngle * -1;
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        float horizontalTilt = inputVector.x * maxHorizontalAngle;
        float verticalTilt = inputVector.y * maxVerticalAngle * -1;


        // rotate camera
        cameraX.transform.localRotation = Quaternion.Euler(0, 90, horizontalTilt);
        cameraY.transform.localRotation = Quaternion.Euler(verticalTilt, 0, 0);

        // follow the sphere
        cameraX.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveSphere();
        MoveCamera();
    }

    // Collide with object
    private void OnTriggerEnter(Collider other)
    {
        // Check if collided with Goal
        if (other.gameObject.CompareTag("Finish"))
        {
            // Victory
            GameManager.instance.Victory();
        }
        // Check if collided with Respawn plane
        else if (other.gameObject.CompareTag("Respawn"))
        {
            // print debug message
            Debug.Log("Respawning...");
            // Respawn
            transform.position = respawnPosition;
            sphere.velocity = Vector3.zero;
            sphere.angularVelocity = Vector3.zero;
        }
    }

}
