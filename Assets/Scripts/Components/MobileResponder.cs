using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MobileResponder : StateResponder
{
    private NavMeshAgent _navMeshAgent;

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    public override void Respond(GameObject target)
    {
        SetDestination(target.transform.position);
    }

    public void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
 
    }

    public float GetPathDistance(Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        _navMeshAgent.CalculatePath(destination, path);

        float distance = 0.0f;

        if ((path.status != NavMeshPathStatus.PathInvalid) && (path.corners.Length > 1))
        {
            for (int i = 1; i < path.corners.Length; ++i)
            {
                distance += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }
        }

        return distance;
    }

}
