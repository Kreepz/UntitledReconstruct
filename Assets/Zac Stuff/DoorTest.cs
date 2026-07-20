using UnityEngine;

public class DoorTest : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnInteract()
    {
        Debug.Log("Door opened");
    }

    public void OnInteract(PlayerGunSystem playerContext)
    {
        throw new System.NotImplementedException();
    }
}
