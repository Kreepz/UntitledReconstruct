using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody _rb;
    [SerializeField] private Transform headPivot;
    private Camera _playerCamera;
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

        if(Camera.main)
        {
            _playerCamera = Camera.main;
            _playerCamera.transform.SetParent(headPivot, false);
            _playerCamera.transform.localPosition = Vector3.zero;
            _playerCamera.transform.localRotation = Quaternion.identity;
            
        }
        GameStateManager.BroadcastPause += ProcessPauseState;
        //if (Camera.main) _playerCamera = Camera.main.transform; //transform.Find("Camera"); 
        StartPlayer();
    }

    public void StartPlayer()
    {
        EngageCamera(true);
    }

    private void OnDestroy()
    {
        GameStateManager.BroadcastPause -= ProcessPauseState;
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        if (GameStateManager.GamePaused) return;
        //Looking
        //camMovement += lookInput;
        transform.rotation = Quaternion.Euler(0, _camMovement.x * mouseSensitivity, 0);
        headPivot.localRotation = Quaternion.Euler(Mathf.Clamp(-_camMovement.y * mouseSensitivity, -80f, 80f), 0, 0);

        //Movement
        if (!_inAir)
        {
            Vector3 targetDirection = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            Vector3 velocity = targetDirection * moveSpeed;
            velocity.y = _rb.linearVelocity.y;
            _rb.linearVelocity = velocity;
        }
    }

    void EngageCamera(bool toggle)
    {
        if (toggle)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    
    void ProcessPauseState(bool toggle)
    {
        if(!toggle && GameStateManager.CurrentState != GameStates.Running) return;
        EngageCamera(!toggle);
    }
    
    public void OnJump(InputAction.CallbackContext obj)
    {
        if (GameStateManager.GamePaused) return;
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

        if (GameStateManager.GamePaused) return;
        _moveInput = obj.ReadValue<Vector2>();
        //Raycast and see what u interacted with if something then fire the objects interact function
    }

    public void OnCameraMove(InputAction.CallbackContext obj)
    {
        if (obj.canceled)
        {
            _camMovement = Vector2.zero;
            return;
        }
        
        if (GameStateManager.GamePaused) return;
        _camMovement += obj.ReadValue<Vector2>();
        //camMovement.y = Mathf.Clamp(-camMovement.y * mouseSensitivity, -80f, 80f);
    }

    public void OnPause(InputAction.CallbackContext obj)
    {
        if (obj.started)
        { 
            if (GameStateManager.CurrentState == GameStates.Running)
            {
                GameStateManager.SetState(GameStates.Paused);
            }
            else if (GameStateManager.GamePaused)
            {
                GameStateManager.SetState(GameStates.Running);
            }
        }
    }
}
