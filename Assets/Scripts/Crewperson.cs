using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crewperson : MonoBehaviour
{
	[SerializeField]
	private List<ProficiencyType> _proficiencies = new List<ProficiencyType>();

	// A dictionary of needs and their values, with a serialized list backing it for editor support.
	private Dictionary<CrewNeed, float> _needs = new Dictionary<CrewNeed, float>();
	[SerializeField]
	private NeedsList _needsList;
	[SerializeField]
	private float _initialNeedValue = 0.5f;


	[SerializeField]
	private float _baseDecayRate = 0.01f;
	[SerializeField]
	private float _baseReplenishRate = 0.1f;

	[SerializeField]
	private float _needsChangeCooldown = 1.0f;
	private float _needsChangeTimer = -1.0f;

	[SerializeField]
	private float _destinationRangeTolerance = 0.5f;
	[SerializeField]
	private float _canMove = -1.0f;

	[SerializeField]
	private CrewPositionNode _assignedPositionNode;
	[SerializeField]
	private PositionNodeSet _crewPositionNodes;
	[SerializeField]
	private NavMeshAgent _navMeshAgent;
	[SerializeField]
	private CrewNeed _priorityNeed; //All crewpersons should start with Standby Need

    void Start()
	{
		// Initialize the needs dictionary
		foreach (CrewNeed need in _needsList.Items)
		{
			//cull needs this crewperson cannot satisfy
			if (!_proficiencies.Contains(need.RequiredProficiency)) continue;

			_needs.Add(need, _initialNeedValue);
		}

		if (_priorityNeed != null)
        {
			_canMove = 1.0f - _priorityNeed.NeedTolerance;
        }

		CalculatePriorities();
		FindAvailableNode(_priorityNeed);
		SetDestination();
	}

	void Update()
	{
		if (_canMove >= 1.0f - _priorityNeed.NeedTolerance && TargetIsReached())
		{
			CalculatePriorities();
			FindAvailableNode(_priorityNeed);
			SetDestination();
		}

		CalculateCanMove();

	
		if (Time.time > _needsChangeTimer)
		{
			_needsChangeTimer = Time.time + _needsChangeCooldown;
			ProcessNeedsChange();
		}
	}

	private void CalculatePriorities()
	{
		foreach (KeyValuePair<CrewNeed,float> need in _needs.OrderBy(n => n.Key.Priority))
        {
			//Debug.Log($"Checking {need.Key.name} at {need.Key.Priority}. Is {need.Value} < {_priorityNeed.NeedTolerance}?");
			if (need.Value < need.Key.NeedTolerance)
			{
				_priorityNeed = need.Key;
				return;
			}
		}
	}

	private void CalculateCanMove()
	{
		// Maintain the _canMove variable so that the crewperson remains on station until their need is met
		_canMove = _needs[_priorityNeed];
	}

	private bool TargetIsReached()
	{
		float distance = Vector3.Distance(transform.position, _assignedPositionNode.transform.position);
		if (distance >= _destinationRangeTolerance)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	void FindAvailableNode(CrewNeed type)
	{
		CrewPositionNode positionNode = _crewPositionNodes.Items.Where(node => node.NeedSatisfied == type && node.IsAvailable).FirstOrDefault();
		if (positionNode == null)
		{
			//Debug.LogWarning("No available node found!");
			return;
		}

		if (positionNode.NeedSatisfied == _assignedPositionNode?.NeedSatisfied) return;

		positionNode.IsAvailable = false;
		if (_assignedPositionNode != null)
		{
			_assignedPositionNode.IsAvailable = true;
		}
		_assignedPositionNode = positionNode;
		//Debug.Log($"{gameObject.name} assigned to {_assignedPositionNode.name}");
	}

	private void SetDestination()
	{
		if (_navMeshAgent.destination != _assignedPositionNode.transform.position)
			_navMeshAgent.destination = _assignedPositionNode.transform.position;
	}

	private void ProcessNeedsChange()
	{
		float distance = Vector3.Distance(transform.position, _assignedPositionNode.transform.position);

		List<CrewNeed> keys = new List<CrewNeed>(_needs.Keys);
		foreach (CrewNeed needKey in keys)
		{
			float newValue = _needs[needKey];
			if (needKey == _assignedPositionNode.NeedSatisfied && TargetIsReached())
			{
				newValue += _baseReplenishRate * needKey.ReplenishmentModifier;
			}
			else
			{
				newValue -= _baseDecayRate * needKey.DecayModifier;
			}

			newValue = Mathf.Clamp(newValue, 0.0f, 1.0f);
			_needs[needKey] = newValue;
			//Debug.Log($"{this.name} {needKey.name} = {_needs[needKey]}");
		}
	}
}
