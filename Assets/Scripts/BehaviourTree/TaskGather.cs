using UnityEngine;

using BehaviorTree;

public class TaskGather : Node
{
    UnitManager _manager;

    public TaskGather(UnitManager manager) : base()
    {
        _manager = manager;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("Gathering...");
        object currentTarget = GetData("currentTarget");
        _manager.Gather((Transform)currentTarget);
        _state = NodeState.SUCCESS;
        return _state;
    }
}