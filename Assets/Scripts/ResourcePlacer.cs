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
