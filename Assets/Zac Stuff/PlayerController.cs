using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Transform playerCamera;

    private PlayerControls playerControls;

    [Header("Settings")]
    public float moveSpeed;
    public float jumpHeight;
    public float mouseSensitivity;
    public float interactRange;

    //Inputs
    private bool inAir;
    private Vector2 moveInput;
    private Vector2 camMovement;
    private Vector2 lookInput;

    private void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Player.Movement.performed += onMove;
        playerControls.Player.Movement.canceled += onMove;
        playerControls.Player.Camera.performed += onCameraMove;
        playerControls.Player.Jump.performed += onJump;
        playerControls.Player.Interact.performed += onInteract;

        playerControls.Enable();
        playerControls.Player.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
        //interact.action.started += Interact;

        playerCamera = Camera.main.transform; //transform.Find("Camera"); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Looking
        //camMovement += lookInput;
        transform.rotation = Quaternion.Euler(0, camMovement.x * mouseSensitivity, 0);
        playerCamera.localRotation = Quaternion.Euler(Mathf.Clamp(-camMovement.y * mouseSensitivity, -80f, 80f), 0, 0);

        //Movement
        if (!inAir)
        {
            Vector3 targetDirection = transform.right * moveInput.x + transform.forward * moveInput.y;
            Vector3 velocity = targetDirection * moveSpeed;
            velocity.y = rb.linearVelocity.y;
            rb.linearVelocity = velocity;
        }
        else
        {
            //Air Strafing probably

            //Landing
            if (Physics.Raycast(transform.position, Vector3.down, 1.5f))
            {
                inAir = false;
                Debug.Log("Landed");
            }
        }
    }

    private void onJump(InputAction.CallbackContext obj)
    {
        //Jumping 
        if (Physics.Raycast(transform.position, Vector3.down, 1.5f))
        {
            inAir = true;
            rb.linearVelocity += Vector3.up * jumpHeight ;
            //Debug.Log("Jumped");
            Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.green, 5, true);
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.red, 5, true);
        }
    }

    //
    private void onInteract(InputAction.CallbackContext obj)
    {
        Debug.DrawRay(playerCamera.position, playerCamera.TransformDirection(Vector3.forward) * interactRange, Color.red, 15, true);

        RaycastHit hit;

        //Raycast and see what u interacted with if something then fire the objects interact functuion    
        if (Physics.Raycast(playerCamera.position, playerCamera.TransformDirection(Vector3.forward), out hit, interactRange))
        {
            Debug.Log("Interacted with:" + hit.transform);
            hit.transform.SendMessage("onInteract", SendMessageOptions.DontRequireReceiver);
        }
    }
    //*/

    private void onMove(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            moveInput = Vector2.zero;
            return;
        }

        moveInput = obj.ReadValue<Vector2>();
        //Raycast and see what u interacted with if something then fire the objects interact functuion
    }

    private void onCameraMove(InputAction.CallbackContext obj)
    {
        camMovement += obj.ReadValue<Vector2>();
        //camMovement.y = Mathf.Clamp(-camMovement.y * mouseSensitivity, -80f, 80f);
    }


}
