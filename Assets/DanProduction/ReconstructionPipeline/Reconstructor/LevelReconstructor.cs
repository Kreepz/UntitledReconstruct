using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class LevelReconstructor
{
    LevelMetadata levelData;
    LevelCollection levelCollection;

    public async Task<TaskResults> ReconstructLevel(LevelMetadata newLevelData)
    {
        TaskResults results = new();
        levelData = newLevelData;
        levelCollection = ContentManager.GetLevelCollection(newLevelData);
        if (levelCollection == null)
        {
            results.Errors.Add("Failed to load level collection");
            results.SubmitResults(false, "Failed to construct level");
            return results;
        }

        foreach (LevelObject levelObject in levelCollection.Children)
        {
            var instantiatedObject = await ConstructLevelObject(levelObject);
            if (!instantiatedObject)
            {
                results.Warnings.Add($"Failed to construct asset : {levelObject.AssetKey}");
            }
        }
        
        results.SubmitResults(true, "Successfully reconstructed level");
        return results;
    }

    public async Task ReloadLevel()
    {
        if (levelCollection == null)
        {
        }
    }
    
    async Task<GameObject> ConstructLevelObject(LevelObject levelObject, Transform parent = null)
    {
        GameObject instance;
        if (levelObject.AssetKey != "")
        {
            instance = await InstantiateAsset(levelObject.AssetKey);
            if (!instance) return instance;
        }
        else instance = new GameObject("Group node");
        
        instance.transform.SetParent(parent);
        
        //Apply transforms
        instance.transform.localPosition = levelObject.Position;
        instance.transform.localRotation = levelObject.Rotation;
        instance.transform.localScale = levelObject.Scale;
        
        //register possible context
        
        
        //Recursively reconstruct children
        if (levelObject.Children is { Count: > 0 })
        {
            foreach (LevelObject child in levelObject.Children)
            {
                await ConstructLevelObject(child, instance.transform);
            }
        }
        
        return instance;
    }

    async Task<GameObject> InstantiateAsset(string assetKey)
    {
        AsyncOperationHandle<GameObject> handle =
            Addressables.InstantiateAsync(assetKey);
        
        return await handle.Task;
    }
    
    

}
