using UnityEngine;

using BehaviorTree;
using System.Collections.Generic;

public class TaskTrySetDestinationOrTarget : Node
{
    CharacterManager _manager;

    private Ray _ray;
    private RaycastHit _raycastHit;
    private const float _samplingRange = 48f; //increments of 12
    private const float _samplingRadius = 7.2f; //increments of 1.8

    public TaskTrySetDestinationOrTarget(CharacterManager manager) : base()
    {
        _manager = manager;
    }

    public override NodeState Evaluate()
    {
        if (_manager.IsSelected && Input.GetMouseButtonUp(1))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(_ray, out _raycastHit, 1000f, Globals.UNIT_MASK))
            {
                UnitManager um = _raycastHit.collider.GetComponent<UnitManager>();
                if (um != null)
                {
                    // assign the current target transform
                    Parent.Parent.SetData("currentTarget", _raycastHit.transform);
                    if (_manager.SelectIndex == 0)
                    {
                        List<Vector2> targetOffsets = _ComputeFormationTargetOffsets();
                        EventManager.TriggerEvent("TargetFormationOffsets", targetOffsets);
                    }
                    _state = NodeState.SUCCESS;
                    return _state;
                }
            }

            else if (Physics.Raycast(_ray, out _raycastHit, 1000f, Globals.TERRAIN_LAYER_MASK))
            {
                if (_manager.SelectIndex == 0) //TODO the unit manager being selected is far greater than index 0
                {
                    List<Vector3> targetPositions = _ComputeFormationTargetPositions(_raycastHit.point);
                    EventManager.TriggerEvent("TargetFormationPositions", targetPositions);
                }
                _state = NodeState.SUCCESS;
                return _state;
            }
        }
        _state = NodeState.FAILURE;
        return _state;
    }

    private List<Vector2> _ComputeFormationTargetOffsets()
    {
        int nSelectedUnits = Globals.SELECTED_UNITS.Count;
        List<Vector2> offsets = new List<Vector2>(nSelectedUnits);
        // leader unit goes to the exact target point
        offsets.Add(Vector2.zero);
        if (nSelectedUnits == 1) // (abort early if no other unit is selected)
            return offsets;

        // next units have offsets computed with a Poisson disc sampling
        offsets.AddRange(Utils.SampleOffsets(
            nSelectedUnits - 1, _samplingRadius, _samplingRange * Vector2.one));
        return offsets;
    }

    private List<Vector3> _ComputeFormationTargetPositions(Vector3 hitPoint)
    {
        int nSelectedUnits = Globals.SELECTED_UNITS.Count;
        List<Vector3> positions = new List<Vector3>(nSelectedUnits);
        // leader unit goes to the exact target point
        positions.Add(hitPoint);
        if (nSelectedUnits == 1) // (abort early if no other unit is selected)
            return positions;

        // next units have positions computed with a Poisson disc sampling
        positions.AddRange(Utils.SamplePositions(
            nSelectedUnits - 1, _samplingRadius,
            _samplingRange * Vector2.one, hitPoint));
        return positions;
    }

    public void SetFormationTargetOffset(List<Vector2> targetOffsets)
    {
        int i = _manager.SelectIndex;
        if (i < 0) return; // (unit is not selected anymore)
        ClearData("destinationPoint");
        Parent.Parent.SetData("currentTargetOffset", targetOffsets[i]);
    }

    public void SetFormationTargetPosition(List<Vector3> targetPositions)
    {
        int i = _manager.SelectIndex;
        if (i < 0) return; // (unit is not selected anymore)
        ClearData("currentTarget");
        ClearData("currentTargetOffset");
        Parent.Parent.SetData("destinationPoint", targetPositions[i]);
    }
}