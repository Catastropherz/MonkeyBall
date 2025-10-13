using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// Control the sphere movement and camera
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
    private float maxHorizontalAngle = 20.0f;

    private Vector3 respawnPosition;

    void Start()
    { 
        // Enable gyroscope
        Input.gyro.enabled = true;

        // Save respawn position
        respawnPosition = transform.position;
    }
    
    // Movement function
    void MoveSphere()
    {
        // set input values
        if (GameManager.instance != null && (GameManager.instance.GetControlMode() == ControlMode.JOYSTICK_DYNAMIC || GameManager.instance.GetControlMode() == ControlMode.JOYSTICK_FIXED))
        {
            // joystick input
            Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
            horizontalInput = inputVector.x * 1.5f;
            verticalInput = inputVector.y;
        }
        else
        {
            // gyroscope input
            horizontalInput = Input.gyro.rotationRateUnbiased.y * 3;
            verticalInput = Input.gyro.rotationRateUnbiased.x * 6;

            // invert vertical input
            verticalInput *= -1;
        }

        float forceMultiplier = moveSpeed * Time.fixedDeltaTime;

        moveDirection = (cameraX.transform.forward * verticalInput * forceMultiplier) + 
                        (cameraY.transform.right * horizontalInput * forceMultiplier);

        // add force to sphere
        sphere.AddForce(moveDirection);

    }

    // Camera function based on input
    void MoveCamera()
    {
        // set input values
        float horizontalTilt, verticalTilt;

        if (GameManager.instance != null && (GameManager.instance.GetControlMode() == ControlMode.JOYSTICK_DYNAMIC || GameManager.instance.GetControlMode() == ControlMode.JOYSTICK_FIXED))
        {
            // joystick input
            // get input vector from joystick
            Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
            horizontalTilt = inputVector.x * maxHorizontalAngle * 1.5f;
            verticalTilt = inputVector.y * maxVerticalAngle * -1;
        }
        else
        {
            Input.gyro.enabled = true;

            Quaternion attitude = Input.gyro.attitude;

            // Remap the rotation from the device to world space
            Quaternion mappedRotation = new Quaternion(attitude.x, attitude.z, attitude.y, -attitude.w);

            // Extract Euler angles
            Vector3 angles = mappedRotation.eulerAngles;

            // Normalize 0-360 angles to -180 to 180 for easier clamping
            float roll = (angles.z > 180) ? angles.z - 360 : angles.z;
            float pitch = (angles.x > 180) ? angles.x - 360 : angles.x;

            // gyroscope input
            horizontalTilt = Mathf.Clamp(roll, -maxHorizontalAngle, maxHorizontalAngle);
            verticalTilt = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);
        }

        // rotate camera

        // Apply some smoothing to the camera rotation
        Quaternion targetX = Quaternion.Euler(0, 90, horizontalTilt);
        Quaternion targetY = Quaternion.Euler(verticalTilt, 0, 0);

        // Camera rotation
        cameraX.transform.localRotation = Quaternion.Lerp(cameraX.transform.localRotation, targetX, Time.deltaTime * 10f);
        cameraY.transform.localRotation = Quaternion.Lerp(cameraY.transform.localRotation, targetY, Time.deltaTime * 10f);


        // follow the sphere
        cameraX.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        MoveCamera();
    }

    // FixedUpdate is called independent of frame rate
    void FixedUpdate()
    {
        MoveSphere();
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

    //Reset gyroscope
    public void ResetGyroscope()
    {
        // Disable and re-enable gyroscope to reset
        Input.gyro.enabled = false;
    }

}
