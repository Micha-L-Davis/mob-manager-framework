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
	private NavMeshAgent _navMeshAgent;

	[SerializeField]
	private Need _resolvableNeed;
    public Need ResolvableNeed => _resolvableNeed;

    public Transform Transform => this.transform;

	private bool _isAvailable = true;
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }

    private void Awake()
    {
		_mobileSet.Items.Add(this);
    }

    private void OnDestroy()
    {
        _mobileSet.Items.Remove(this);
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
			needyObject.ReplenishNeed(ResolvableNeed);
			return true;
        }

		return false;
    }
}
