using UnityEngine;

public interface IInteractable
{
    void OnInteract();

    void OnInteract(PlayerGunSystem playerContext);
}
