using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerGunSystem : MonoBehaviour
{
    private Transform playerCamera;
    private PlayerControls playerControls;

    [Header("Guns")]
    public GameObject slot1;
    public GameObject slot2;

    [Header("Settings")]
    public float InteractRange;

    private void Awake()
    {
        //playerControls = gameObject.GetComponent<PlayerController>().playerControls;
        //playerControls.Player.Interact.performed += onInteract;

        playerCamera = Camera.main.transform;
    }

    /*
    private void onInteract(InputAction.CallbackContext obj)
    {
        Debug.DrawRay(playerCamera.position, playerCamera.TransformDirection(Vector3.forward) * InteractRange, Color.red, 15, true);

        RaycastHit hit;

        //Raycast and see what u interacted with if something then fire the objects interact functuion    
        if (Physics.Raycast(playerCamera.position, playerCamera.TransformDirection(Vector3.forward), out hit,InteractRange))
        {
            Debug.Log("Interacted with:" + hit.transform);
            hit.transform.SendMessage("onInteract", SendMessageOptions.DontRequireReceiver);
        }
    }
    //*/
}