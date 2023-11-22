using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Needs))]
public class Crewperson : MonoBehaviour, INeedResolver
{
	[SerializeField]
	private List<ProficiencyType> _proficiencies = new List<ProficiencyType>();
	[SerializeField]
	private CrewpersonSet _crewpersonSet;

	[SerializeField]
	private Needs _needs;

	[SerializeField]
	private NavMeshAgent _navMeshAgent;

	[SerializeField]
	private Need _resolvableNeed;
    public Need ResolvableNeed => _resolvableNeed;

    public Transform Transform => this.transform;

    public bool IsAvailable { get => CanMove(); set{ return; } }

    private void Awake()
    {
		_crewpersonSet.Items.Add(this);
    }

    void Start()
	{
		_needs = gameObject.GetComponent<Needs>();
		SetDestination(_needs.AssignedResolver.Transform.position);
	}

	void Update()
	{
		if (CanMove())
        {
			SetDestination(_needs.AssignedResolver.Transform.position);
        }
	}

    private void OnDestroy()
    {
        _crewpersonSet.Items.Remove(this);
    }

	private bool CanMove()
    {
		return _needs.PriorityNeedValue >= 1.0f - _needs.PriorityNeed.NeedTolerance && !_needs.ResolverInRange();
	}

	private void SetDestination(Vector3 position)
	{
		if (_navMeshAgent.destination != position)
			_navMeshAgent.destination = position;
	}

    public void ResolveNeed(GameObject needyObject)
    {
		SetDestination(needyObject.transform.position);
    }
}
