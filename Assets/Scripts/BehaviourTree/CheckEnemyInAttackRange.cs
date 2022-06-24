using UnityEngine;

using BehaviorTree;

public class CheckEnemyInAttackRange : Node
{
    UnitManager _manager;
    float _attackRange;

    public CheckEnemyInAttackRange(UnitManager manager) : base()
    {
        _manager = manager;
        _attackRange = _manager.Unit.Data.attackRange;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        if (currentTarget == null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }

        Transform target = (Transform)currentTarget;

        // (in case the target object is gone - for example it died
        // and we haven't cleared it from the data yet)
        if (!target)
        {
            Parent.ClearData("currentTarget");
            _state = NodeState.FAILURE;
            return _state;
        }

        //TODO: scale is multipled by 5
        Vector3 s = target.Find("Mesh").localScale / 10;
        float targetSize = Mathf.Max(s.x, s.z);

        float distance = Vector3.Distance(_manager.transform.position, target.position);
        bool isInRange = (distance - targetSize) <= _attackRange;
        //bool isInRange = distance <= _attackRange;
        _state = isInRange ? NodeState.SUCCESS : NodeState.FAILURE;
        return _state;
    }
}