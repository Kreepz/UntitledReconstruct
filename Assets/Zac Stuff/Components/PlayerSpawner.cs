using UnityEngine;

public class PlayerSpawner : ReconstructableBehaviour
{
    public override string BehaviourID => "Bus stop spawner";
    [SerializeField] GameObject playerReference;
    [SerializeField] Transform spawnPoint;
     GameObject _playerInstance;
    
    public override void OnLevelLoaded()
    {
    }

    public override void OnLevelStart()
    {
        if (!_playerInstance)
            _playerInstance = Instantiate(playerReference, spawnPoint);
        
        _playerInstance.transform.localPosition = Vector3.zero;
        _playerInstance.transform.localRotation = Quaternion.identity;
        
        _playerInstance.GetComponent<PlayerController>().StartPlayer();
    }

    public override BehaviourContext CompileContext()
    {
        return null;
    }
    public override void ImportContext(BehaviourContext ctx)
    {
    }
}
