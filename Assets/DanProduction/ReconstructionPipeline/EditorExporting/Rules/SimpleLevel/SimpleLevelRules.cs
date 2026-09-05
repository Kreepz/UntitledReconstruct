using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleLevelRules", menuName = "ExtractableLevelRules/SimpleLevel")]
public class SimpleLevelRules : ExtractableLevelRules
{
    [SerializeField] GameObject playerSpawner;
    
    public override TaskResults ValidateHierarchy(GameObject rootObject)
    {
        TaskResults results = new();
        
        int playerSpawnerCount = 0;
        
        foreach (Transform transformChild in rootObject.GetComponentsInChildren<Transform>())
        {
            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(transformChild.gameObject);
            if(sourcePrefab == playerSpawner)
                playerSpawnerCount++;
        }

        if (playerSpawnerCount == 1)
        {
            results.SubmitResults(true, "Validation passed");
            Debug.Log("Validation passed");
        }
        else
        {
            string error = playerSpawnerCount == 0 ? "No player spawners found" : $"Too many player spawners found: {playerSpawnerCount}";
            results.Errors.Add(error);
            results.SubmitResults(false, "Validation failed");
            Debug.LogError($"Validation failed because of: \n" +
                      $"{error}");
        }
        
        //results.SubmitResults(false, "Preventing validation pass");
        return results;
    }
}
