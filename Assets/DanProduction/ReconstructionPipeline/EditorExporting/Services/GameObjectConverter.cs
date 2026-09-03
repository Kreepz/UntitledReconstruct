using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

public static class GameObjectConverter
{
    public static LevelObject ConvertToLevelObject(GameObject convertingObject)
    {
        //Resolve type
        LevelObjectType objectType = convertingObject switch
        {
            var o when convertingObject.CompareTag("ExtractableObject") => LevelObjectType.ExtractableObject,
            var o when convertingObject.CompareTag("GroupingNode") => LevelObjectType.GroupingNode,
            _ => LevelObjectType.Ignore
        };
        
        if(objectType == LevelObjectType.Ignore)return null;
        
        //Resolve key
        string assetKey;
        if (objectType is LevelObjectType.ExtractableObject)
        {
            string foundKey = GetAddressableKey(convertingObject);
            if (foundKey == null)
            {
                Debug.LogError("This object is not inside the registry");
                return null;
            }
            assetKey = foundKey;
        }
        else
            assetKey = "";
        
        //Resolve transforms
        Vector3 objectPosition = convertingObject.transform.localPosition;
        Quaternion objectRotation = convertingObject.transform.localRotation;
        Vector3 objectScale = convertingObject.transform.localScale;
        
        
        //Resolve children
        List<LevelObject> immediateChildren = new List<LevelObject>();
        if (objectType == LevelObjectType.GroupingNode)
        {
            foreach (Transform child in convertingObject.transform)
            {
                LevelObject immediateChild = ConvertToLevelObject(child.gameObject);
                if (immediateChild != null)
                    immediateChildren.Add(immediateChild);
            }
        }
        
        LevelObject convertedObject = new LevelObject
        {
            AssetKey = assetKey,
            Position = objectPosition,
            Rotation = objectRotation,
            Scale = objectScale
        };
        if (immediateChildren.Count > 0)
        {
            convertedObject.Children = immediateChildren;
        }
        
        return convertedObject;
    }

    static string GetAddressableKey(GameObject targetObject)
    {
        GameObject prefab = PrefabUtility.GetCorrespondingObjectFromSource(targetObject);

        if (!prefab)
            return null;
        
        string assetPath = AssetDatabase.GetAssetPath(prefab);
        string guid = AssetDatabase.AssetPathToGUID(assetPath);

        AddressableAssetSettings settings =
            AddressableAssetSettingsDefaultObject.Settings;

        AddressableAssetEntry entry =
            settings.FindAssetEntry(guid);

        if (entry == null)
            return null;
        
        return entry.address;
    }
}
