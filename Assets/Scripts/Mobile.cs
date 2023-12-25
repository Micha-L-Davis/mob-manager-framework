using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Mobile : NeedyObject
{
	[SerializeField]
	internal ResolverSet _mobileSet;

	[SerializeField]
	private NavMeshAgent _navMeshAgent;

    internal override void OnEnable()
    {
		base.OnEnable();

		_mobileSet.Items.Add(this);
    }

    internal override void OnDisable()
    {
		base.OnDisable();

        _mobileSet.Items.Remove(this);
	}

	public void SetDestination(Vector3 position)
	{
		if (_navMeshAgent.destination != position)
			_navMeshAgent.destination = position;
	}

    public override bool ProvideResolution(Needs needyObject)
    {
		if (_navMeshAgent.destination != needyObject.transform.position)
        {
			SetDestination(needyObject.transform.position);
			return false;
        }

		if (_navMeshAgent.remainingDistance < 0.5f)
        {
			foreach (Need need in ResolvableNeeds)
            {
				needyObject.ReplenishNeed(need);
				return true;
            }
        }

		return false;
    }

    public override void SeekResolution(INeedResolver resolver)
    {
		Debug.Log($"{gameObject.name} needs to get to {resolver.Transform.gameObject.name} at {resolver.Transform.position}!");
		SetDestination(resolver.Transform.position);
	}
}
