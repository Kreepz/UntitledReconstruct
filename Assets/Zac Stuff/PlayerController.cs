using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    private Transform _playerCamera;
    public PlayerInput playerInput;
    
    [Header("Settings")]
    public float moveSpeed;
    public float jumpHeight;
    public float mouseSensitivity;
    public float interactRange;

    //Inputs
    private bool _inAir;
    private Vector2 _moveInput;
    private Vector2 _camMovement;
    private Vector2 _lookInput;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = gameObject.GetComponent<Rigidbody>();

        if (Camera.main) _playerCamera = Camera.main.transform; //transform.Find("Camera"); 

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Looking
        //camMovement += lookInput;
        transform.rotation = Quaternion.Euler(0, _camMovement.x * mouseSensitivity, 0);
        _playerCamera.localRotation = Quaternion.Euler(Mathf.Clamp(-_camMovement.y * mouseSensitivity, -80f, 80f), 0, 0);

        //Movement
        if (!_inAir)
        {
            Vector3 targetDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            Vector3 velocity = targetDirection * moveSpeed;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
    }

    public void OnJump(InputAction.CallbackContext obj)
    {
        //Jumping 
        if (Physics.Raycast(transform.position, Vector3.down, 1.1f))
        {
            _rb.linearVelocity += Vector3.up * jumpHeight ;
            //Debug.Log("Jumped");
            Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.green, 5, true);
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.red, 5, true);
        }
    }

    public void OnMove(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            _moveInput = Vector2.zero;
            return;
        }

        _moveInput = obj.ReadValue<Vector2>();
        //Raycast and see what u interacted with if something then fire the objects interact function
    }

    public void OnCameraMove(InputAction.CallbackContext obj)
    {
        _camMovement += obj.ReadValue<Vector2>();
        //camMovement.y = Mathf.Clamp(-camMovement.y * mouseSensitivity, -80f, 80f);
    }
}
