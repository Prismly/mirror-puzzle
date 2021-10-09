using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorTileLibrary : MonoBehaviour
{
    [SerializeField]
    private GameObject[] prefabs;

    private static ActorTileLibrary singleton = null;
    public static ActorTileLibrary GetInstance()
    {
        if(singleton == null)
        {
            singleton = FindObjectOfType(typeof(ActorTileLibrary)) as ActorTileLibrary;
        }
        return singleton;
    }

    public GameObject GetPrefab(int id)
    {
        if (id >= 0 && id < prefabs.Length)
        {
            return prefabs[id];
        }
        Debug.Log("PrefabLibrary tried to return a prefab at an index that does not exist.");
        return null;
    }
}
