using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorLibrary : MonoBehaviour
{
    /** The characters that have associated GameObject prefabs in the values array.
     * Used to populate the prefabs Dictionary, and intended to be "initialized" in the editor. */
    [SerializeField] private char[] keys;
    /** The GameObject prefabs that have associated character "codes" in the keys array.
     * Used to populate the prefabs Dictionary, and intended to be "initialized" in the editor. */
    [SerializeField] private GameObject[] values;
    /** A Dictionary that knows which prefabs correspond to which text characters when reading a level in from a text file.
     * For example, if reading a 'W' means creating a Wall actor in that square, that relationship will be stored here.*/
    private static Dictionary<char, GameObject> prefabs = new Dictionary<char, GameObject>();

    /**
     * Populates the prefabs dictionary with the key-value pairs specified in the Unity editor.
     */
    public void Start()
    {
        for (int i = 0; i < keys.Length; i++)
        {
            prefabs.Add(keys[i], values[i]);
        }
    }

    /**
     * Given a character value, searches the prefabs Dictionary and returns the prefab that has
     * that given value as its key. If no such prefab exists, return null.
     * @param id - the char key to search the Dictionary for.
     * @return the GameObject prefab with the given key, or null if it is not in the Dictionary.
     */
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
