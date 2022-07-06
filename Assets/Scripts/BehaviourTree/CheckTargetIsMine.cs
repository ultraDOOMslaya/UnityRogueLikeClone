using UnityEngine;

using BehaviorTree;

public class CheckTargetIsMine : Node
{
    private int _myPlayerId;
    private UnitManager _myUnitManager;

    public CheckTargetIsMine(UnitManager manager) : base()
    {
        _myPlayerId = GameManager.instance.gamePlayersParameters.myPlayerId;
        _myUnitManager = manager;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        HarvestResourceManager hrm = ((Transform)currentTarget).GetComponent<HarvestResourceManager>();
        if (hrm != null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }
        UnitManager um = ((Transform)currentTarget).GetComponent<UnitManager>();
        if (um == null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }
        //_state = um.Unit.Owner == _myPlayerId ? NodeState.SUCCESS : NodeState.FAILURE;
        _state = um.Unit.Owner == _myUnitManager.Unit.Owner ? NodeState.SUCCESS : NodeState.FAILURE;
        Debug.Log("target is mine state: " + _state);
        Debug.Log("target identifier: " + um.Unit.Data.unitName);
        return _state;
    }
}