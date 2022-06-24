using System.Collections.Generic;

using BehaviorTree;

[UnityEngine.RequireComponent(typeof(CharacterManager))]
public class CharacterBT : Tree
{
    CharacterManager manager;
    private TaskTrySetDestinationOrTarget _trySetDestinationOrTargetNode;

    private void Awake()
    {
        manager = GetComponent<CharacterManager>();
    }

    private void OnEnable()
    {
        EventManager.AddListener("TargetFormationOffsets", _OnTargetFormationOffsets);
        EventManager.AddListener("TargetFormationPositions", _OnTargetFormationPositions);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("TargetFormationOffsets", _OnTargetFormationOffsets);
        EventManager.RemoveListener("TargetFormationPositions", _OnTargetFormationPositions);
    }

    private void _OnTargetFormationOffsets(object data)
    {
        List<UnityEngine.Vector2> targetOffsets = (List<UnityEngine.Vector2>)data;
        _trySetDestinationOrTargetNode.SetFormationTargetOffset(targetOffsets);
    }

    private void _OnTargetFormationPositions(object data)
    {
        List<UnityEngine.Vector3> targetPositions = (List<UnityEngine.Vector3>)data;
        _trySetDestinationOrTargetNode.SetFormationTargetPosition(targetPositions);
    }

    protected override Node SetupTree()
    {
        Node _root;

        // prepare our subtrees...
        _trySetDestinationOrTargetNode = new TaskTrySetDestinationOrTarget(manager);
        Sequence trySetDestinationOrTargetSequence = new Sequence(new List<Node> {
            new CheckUnitIsMine(manager),
            _trySetDestinationOrTargetNode,
        });

        Sequence moveToDestinationSequence = new Sequence(new List<Node> {
            new CheckHasDestination(),
            new TaskMoveToDestination(manager),
        });


        Selector attackOrBuildSelector = new Selector();
        if (manager.Unit.Data.attackDamage > 0)
        {
            Sequence attackSequence = new Sequence(new List<Node> {
                new Inverter(new List<Node>
                {
                    new CheckTargetIsMine(manager),
                }),
                new CheckEnemyInAttackRange(manager),
                new Timer(
                    manager.Unit.Data.attackRate,
                    new List<Node>()
                    {
                        new TaskAttack(manager)
                    }
                ),
            });

            attackOrBuildSelector.Attach(attackSequence);
            attackOrBuildSelector.Attach(new TaskFollow(manager));
        }


        Sequence moveToTargetSequence = new Sequence(new List<Node> {
            new CheckHasTarget(),
            new Selector(new List<Node> {
                attackOrBuildSelector,
                new TaskFollow(manager),
            })
        });

        // ... then stitch them together under the root
        _root = new Selector(new List<Node> {
            new Parallel(new List<Node> {
                trySetDestinationOrTargetSequence,
                new Selector(new List<Node>
                {
                    moveToDestinationSequence,
                    moveToTargetSequence,
                }),
            }),
            new CheckEnemyInFOVRange(manager),
        });

        return _root;
    }
}