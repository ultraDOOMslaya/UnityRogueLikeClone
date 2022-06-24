using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ResourceValue
{
    public string code = "";
    public int amount = 0;

    public ResourceValue(string code, int amount)
    {
        this.code = code;
        this.amount = amount;
    }
}
