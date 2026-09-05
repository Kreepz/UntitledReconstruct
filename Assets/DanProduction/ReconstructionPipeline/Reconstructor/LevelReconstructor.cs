using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class LevelReconstructor
{
    LevelMetadata levelData;
    LevelCollection levelCollection;

    List<ReloadableObject> behaviouralCollection = new();
    
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

    public Task<TaskResults> ReloadLevel()
    {
        TaskResults results = new();
        if (behaviouralCollection is { Count: > 0 })
        {
            foreach (ReloadableObject obj in behaviouralCollection)
            {
                obj.ResetTransform();
            }
        }
        results.SubmitResults(true, "Successfully reloaded level");
        return Task.FromResult(results);
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
        if (levelObject.BehaviourContexts is { Count: > 0 })
        {
            ReconstructableBehaviour[] behaviours = 
                instance.GetComponents<ReconstructableBehaviour>();

            foreach (BehaviourContext ctx in levelObject.BehaviourContexts)
            {
                ReconstructableBehaviour match = 
                    Array.Find(behaviours, b => 
                        b.BehaviourID == ctx.BehaviourID);

                if (match)
                    match.ImportContext(ctx);
            }

            ReloadableObject obj = new(instance)
            {
                Position = levelObject.Position,
                Rotation = levelObject.Rotation,
                Scale = levelObject.Scale,
            };
            behaviouralCollection.Add(obj);
        }
        else if (instance.GetComponent<ReconstructableBehaviour>())
        {
            ReloadableObject obj = new(instance)
            {
                Position = levelObject.Position,
                Rotation = levelObject.Rotation,
                Scale = levelObject.Scale,
            };
            
            behaviouralCollection.Add(obj);
        }
        
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


    public void LoadBehaviourComponents()
    {
        foreach (ReloadableObject obj in behaviouralCollection)
        {
            obj.Reload();
        }
    }
    
    public void StartBehaviourComponents()
    {
        foreach (ReloadableObject obj in behaviouralCollection)
        {
            obj.Restart();
        }
    }
}
