using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class Resource
{
    public Transform transform;

    protected ResourceData _data;
    protected int _currentYield;

    public Resource(ResourceData data)
    {
        _data = data;
        _currentYield = data.resourceYield;
    }

    public ResourceData Data { get => _data;  }
    public int Yield { get => _currentYield; set => _currentYield = value; }
}

public class ResourcePlacer : MonoBehaviour
{
    public static ResourcePlacer instance;

    private void Start()
    {
        instance = this;

    }

    public void SpawnResource(Vector3 position)
    {

    }
}
