using UnityEngine;

public class Gun : MonoBehaviour
{
    private GameObject player;
    private PlayerGunSystem inventory;
    private bool dropped;

    [Header("Gun Stats")]
    public float damage;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        inventory = player.GetComponent<PlayerGunSystem>();
    }


    public void onInteract()
    {
        Debug.Log("Gun Interacted With");
        if (inventory.slot1 == null)
        {
            Debug.Log(gameObject.name + "equipped to slot1");
            inventory.slot1 = gameObject;
            transform.SetParent(player.transform.Find("Hand"),false);
            //Set pos + Disable collider
        }
        else if (inventory.slot1 == null)
        {
            Debug.Log(gameObject.name + "equipped to slot1");
            inventory.slot2 = gameObject;
            transform.SetParent(player.transform.Find("Hand"), false);
        }
        else
        {
            Debug.Log("Inventory Slots full");
        }

    }
}
