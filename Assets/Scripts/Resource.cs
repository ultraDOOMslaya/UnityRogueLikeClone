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

        GameObject g = GameObject.Instantiate(data.prefab) as GameObject;
        transform = g.transform;
        transform.GetComponent<HarvestResourceManager>().Initialize(this);
    }

    public ResourceData Data { get => _data;  }
    public int Yield { get => _currentYield; set => _currentYield = value; }

    public static implicit operator Resource(ResourceData v)
    {
        throw new NotImplementedException();
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
<<<<<<< HEAD

    public int MaxYield { get => _data.resourceYield; }
=======
>>>>>>> 7d1c822f120fa7fa5d5fe7ca40d064292e7907f9
}
