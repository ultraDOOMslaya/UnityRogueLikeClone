using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomEventData
{
    public UnitData unitData;
    public BuildingData buildingData;
    public Unit unit;

    public CustomEventData(UnitData unitData)
    {
        this.unitData = unitData;
        this.unit = null;
    }

    public CustomEventData(Unit unit)
    {
        this.unitData = null;
        this.unit = unit;
    }

    public CustomEventData(BuildingData buildingData)
    {
        this.buildingData = buildingData;
    }
}

[System.Serializable]
public class CustomEvent : UnityEvent<CustomEventData> { }