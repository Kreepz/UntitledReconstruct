using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerGunSystem : MonoBehaviour
{
    private Transform _playerCamera;
    [Header("References")]
    public Transform handPivot;
    
    
    public int currentSlot =0 ;
    
    [Header("Guns")]
    public GameObject slot1;
    public GameObject slot2;

    [Header("Settings")] 
    public float interactRange;

    private void Awake()
    {
        //playerControls = gameObject.GetComponent<PlayerController>().playerControls;
        //playerControls.Player.Interact.performed += onInteract;

        if (Camera.main) _playerCamera = Camera.main.transform;
        else Debug.LogError("No camera located");
        
    }

    //
    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if(!ctx.performed) return;
        Debug.Log("Interacted");
        Debug.DrawRay(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward) * interactRange, Color.red, 15, true);

        RaycastHit hit;

        //Raycast and see what u interacted with if something then fire the objects interact function    
        if (Physics.Raycast(_playerCamera.position, _playerCamera.TransformDirection(Vector3.forward), out hit,interactRange))
        {
            Debug.Log("Interacted with:" + hit.transform);
            if (hit.collider.TryGetComponent(out IInteractable interactable))
            {
                interactable.OnInteract();
                interactable.OnInteract(this);
            }
            //hit.transform.SendMessage("onInteract", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void EquipSlot1(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Debug.Log("Equip slot 1");
        
        if (slot1 != null)
        { currentSlot = 1;}
       
    }
    
    public void EquipSlot2(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        Debug.Log("Equip slot 2");
        
        if (slot2 != null)
        { currentSlot = 2;}
    }
    //*/
}