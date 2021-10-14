using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorLibrary : MonoBehaviour
{
    [SerializeField]
    private char[] keys;
    [SerializeField]
    private GameObject[] values;
    private static Dictionary<char, GameObject> prefabs = new Dictionary<char, GameObject>();

    public void Start()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            prefabs.Add(keys[i], values[i]);
        }
    }

    public static GameObject GetPrefab(char id)
    {
        GameObject returnObject;
        if(prefabs.TryGetValue(id, out returnObject))
        {
            return returnObject;
        }
        return null;
    }
}
