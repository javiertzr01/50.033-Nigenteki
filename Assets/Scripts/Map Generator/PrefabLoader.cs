using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Unity.Netcode;

public class PrefabLoader : MonoBehaviour
{
    public static GameObject LoadAndInstantiatePrefab(string key, Vector3 parentTransform)
    {
        AsyncOperationHandle<GameObject> opHandle = Addressables.LoadAssetAsync<GameObject>(key);
        opHandle.WaitForCompletion();

        if (opHandle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = opHandle.Result;
            return Instantiate(prefab, parentTransform, Quaternion.identity);
        }
        else
        {
            Debug.Log("Loading prefab failed");
        }

        return null;
    }

    public static void LoadAndInstantiateNetworkPrefab(string key, Vector2Int parentTransform)
    {
        GameObject netObj = LoadAndInstantiatePrefab(key, new Vector3(parentTransform.x, parentTransform.y, 0));
        netObj.GetComponent<NetworkObject>().Spawn();
    }

    public static List<GameObject> LoadAndInstantiatePrefabs(string[] keys, Vector2Int[] parentTransform)
    {
        List<GameObject> instantiatedPrefabs = new List<GameObject>();
        // List<GameObject> flowersPrefabs = new List<GameObject>();
        GameObject instantiatedPrefab;
        for (int i = 0; i < keys.Length; i++)
        {
            string biome = keys[i].Substring(0,6);
            string item = keys[i].Substring(6);
            // if (item.Substring(Mathf.Max(0, item.Length - 6)) == "Flower")
            // {
            //     instantiatedPrefab = LoadAndInstantiatePrefab(item, new Vector3(parentTransform[i].x, parentTransform[i].y, 0));
                // flowersPrefabs.Add(instantiatedPrefab);
            // }
            // else
            // {
            instantiatedPrefab = LoadAndInstantiatePrefab(keys[i], new Vector3(parentTransform[i].x, parentTransform[i].y, 0));
            instantiatedPrefabs.Add(instantiatedPrefab);
            // }
            if (instantiatedPrefab == null)
            {
                Debug.Log("No instantiated Prefabs");
            }
        }

        // return (flowersPrefabs, instantiatedPrefabs);
        return instantiatedPrefabs;
    }
}
