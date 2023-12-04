using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Needs),typeof(NavMeshAgent))]
public class Mobile : MonoBehaviour, INeedResolver
{
	[SerializeField]
	private MobileSet _mobileSet;
	[SerializeField] 
	private ResolverSet _resolverSet;

	[SerializeField]
	private NavMeshAgent _navMeshAgent;

	[SerializeField]
	private List<Need> _resolvableNeeds;
    public List<Need> ResolvableNeeds => _resolvableNeeds;

    public Transform Transform => this.transform;

	private bool _isAvailable = true;
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }

    private void Awake()
    {
		_mobileSet.Items.Add(this);
		_resolverSet.Items.Add(this);
    }

    private void OnDestroy()
    {
        _mobileSet.Items.Remove(this);
		_resolverSet.Items.Remove(this);
	}

	private void SetDestination(Vector3 position)
	{
		if (_navMeshAgent.destination != position)
			_navMeshAgent.destination = position;
	}

    public bool ResolveNeed(Needs needyObject)
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
}
