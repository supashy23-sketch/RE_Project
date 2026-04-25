using UnityEngine;
using System.Collections.Generic;

public class PlayerKeyHolder : MonoBehaviour
{
    private HashSet<string> collectedKeys = new HashSet<string>();

    public void AddKey(string keyID)
    {
        collectedKeys.Add(keyID);
    }

    public bool HasKey(string keyID)
    {
        return collectedKeys.Contains(keyID);
    }

    public void RemoveKey(string keyID)
    {
        collectedKeys.Remove(keyID);
    }
    public string GetKeyList()
    {
        return collectedKeys.Count == 0 ? "(none)" : string.Join(", ", collectedKeys);
    }
}