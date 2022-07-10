using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Unit", menuName = "Scriptable Objects/Building", order = 1)]
public class UnitData : ScriptableObject
{
    public string code;
    public string unitName;
    public int healthpoints;
    public float attackRange;
    public float fieldOfView;
    public int attackDamage;
    public float attackRate;
<<<<<<< HEAD
    public int harvestRate;
=======
    public int resourceYield;
>>>>>>> 7d1c822f120fa7fa5d5fe7ca40d064292e7907f9
    public GameObject prefab;
    public List<ResourceValue> cost;
    public List<SkillData> skills = new List<SkillData>();

    public bool CanBuy()
    {
        foreach (ResourceValue resource in cost)
        {
            if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
            {
                return false;
            }
        }
        return true;
    }
}