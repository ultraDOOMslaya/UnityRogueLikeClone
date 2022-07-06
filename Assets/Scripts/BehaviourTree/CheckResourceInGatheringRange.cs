using UnityEngine;

using BehaviorTree;

public class CheckResourceInGatheringRange : Node
{
    UnitManager _manager;
    float _attackRange;

    public CheckResourceInGatheringRange(UnitManager manager) : base()
    {
        _manager = manager;
        _attackRange = _manager.Unit.Data.attackRange;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        Debug.Log("Checking for gathering");
        if (currentTarget == null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }

        Transform target = (Transform)currentTarget;
        HarvestResourceManager hrm = target.GetComponent<HarvestResourceManager>();
        if (hrm == null)
        {
            //Target is a unit
            _state = NodeState.FAILURE;
            return _state;
        }
        // (in case the target object is gone - for example it died
        // and we haven't cleared it from the data yet)
        if (!target)
        {
            Parent.ClearData("currentTarget");
            _state = NodeState.FAILURE;
            return _state;
        }

        //TODO: scale is multipled by 5
        //Vector3 s = target.Find("Mesh").localScale / 10;
        //float targetSize = Mathf.Max(s.x, s.z);

        float distance = Vector3.Distance(_manager.transform.position, target.position);
        //TODO: Different sets of mesh have different scales which makes these calculations difficult
        //bool isInRange = (distance - targetSize) <= _attackRange;
        bool isInRange = distance <= _attackRange;
        Debug.Log("In range to gather? " + isInRange);
        _state = isInRange ? NodeState.SUCCESS : NodeState.FAILURE;
        return _state;
    }
}