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
        Debug.Log("Checking if target is mine");
        object currentTarget = Parent.GetData("currentTarget");
        UnitManager um = ((Transform)currentTarget).GetComponent<UnitManager>();
        if (um == null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }
        //_state = um.Unit.Owner == _myPlayerId ? NodeState.SUCCESS : NodeState.FAILURE;
        _state = um.Unit.Owner == _myUnitManager.Unit.Owner ? NodeState.SUCCESS : NodeState.FAILURE;
        return _state;
    }
}