using UnityEngine;
using UnityEngine.AI;

public class CharacterManager : UnitManager
{
    public UnityEngine.AI.NavMeshAgent agent;

    private Character _character;

    public override Unit Unit
    {
        get { return _character; }
        set { _character = value is Character ? (Character)value : null; }
    }

    private void Start()
    {
        _animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        float velocity = agent.velocity.magnitude;
        _animator.SetFloat("Walk", velocity);
    }

    public bool MoveTo(Vector3 targetPosition)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(targetPosition, path);
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            return false;
        }

        agent.destination = targetPosition;

        return true;
    }
}