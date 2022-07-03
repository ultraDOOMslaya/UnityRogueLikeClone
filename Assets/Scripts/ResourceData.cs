using UnityEngine;

[CreateAssetMenu(fileName = "Resource", menuName = "Scriptable Objects/Resource", order = 1)]
public class ResourceData : ScriptableObject
{
    public string code;
    public string resourceName;
    public int resourceYield;
    public bool depleted;
    public GameObject prefab;
}

