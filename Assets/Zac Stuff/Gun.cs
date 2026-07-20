using UnityEngine;

public class Gun : MonoBehaviour, IInteractable
{
    private PlayerGunSystem inventory;
    private bool dropped;

    [Header("Gun Stats")]
    public float damage;

    private void Start()
    {
        //inventory = player.GetComponent<PlayerGunSystem>();
    }


    public void OnInteract()
    {
        
    }

    public void OnInteract(PlayerGunSystem playerContext)
    {
        inventory = playerContext;
        Debug.Log("Gun Interacted With");
        if (playerContext.slot1 == null)
        {
            Debug.Log(gameObject.name + "equipped to slot1");
            playerContext.slot1 = gameObject;
            Debug.Log(inventory.handPivot);
            transform.SetParent(inventory.handPivot,false);
            transform.localPosition = Vector3.zero;
            //Set pos + Disable collider
            
        }
        else if (inventory.slot2 == null)
        {
            Debug.Log(gameObject.name + "equipped to slot1");
            inventory.slot2 = gameObject;
            
            transform.SetParent(inventory.handPivot,false);
            transform.localPosition = Vector3.zero;
        }
        else
        {
            Debug.Log("Inventory Slots full");
        }
    }
}
