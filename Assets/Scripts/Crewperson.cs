using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crewperson : MonoBehaviour
{
	[SerializeField]
	private ProficiencyType _proficiency;

	// A dictionary of needs and their values, with a serialized list backing it for editor support.
	private Dictionary<CrewNeed, float> _needs = new Dictionary<CrewNeed, float>();
	[SerializeField]
	private NeedsList _needsList;
	[SerializeField]
	private float _initialNeedValue = 0.5f;

	[SerializeField]
	private float _needsTolerance = 0.25f;
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
	private CrewActivityType _destinationType;
	[SerializeField]
	private CrewNeed _priorityNeed;

    void Start()
	{
		// Initialize the needs dictionary
		foreach (CrewNeed need in _needsList.Items)
		{
			if (_proficiency != need.RequiredProficiency) continue;

			_needs.Add(need, _initialNeedValue);
		}

		_canMove = 1.0f - _needsTolerance;
		CalculatePriorities();
		AssignPositionNode();
		SetDestination();
	}

	void Update()
	{
		if (_canMove >= 1.0f - _needsTolerance && TargetIsReached())
		{
			CalculatePriorities();
			AssignPositionNode();
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
		_priorityNeed = null;

		foreach (KeyValuePair<CrewNeed,float> need in _needs.OrderBy(n => n.Key.Priority))
        {
			if (need.Value < _needsTolerance)
			{
				_priorityNeed = need.Key;
				_destinationType = need.Key.PositionNodeType;
				return;
			}
		}

		if (_priorityNeed == null)
        {
			_destinationType = CrewActivityType.Idle;
        }
	}

	private void CalculateCanMove()
	{
		// Maintain the _canMove variable so that the crewperson remains on station until their need is met
		if (_priorityNeed != null)
        {
			_canMove = _needs[_priorityNeed];
        }
		else
        {
			_canMove = 1 - _needsTolerance;
        }
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

	private void AssignPositionNode()
	{
		void FindAvailableNode(CrewActivityType type)
		{
			CrewPositionNode positionNode = _crewPositionNodes.Items.Where(node => node.NodeType == type && node.IsAvailable).FirstOrDefault();
			if (positionNode == null)
			{
				//Debug.LogWarning("No available node found!");
				return;
			}

			if (positionNode.NodeType == _assignedPositionNode?.NodeType) return;

			positionNode.IsAvailable = false;
			if (_assignedPositionNode != null)
            {
				_assignedPositionNode.IsAvailable = true;
            }
			_assignedPositionNode = positionNode;
			//Debug.Log($"{gameObject.name} assigned to {_assignedPositionNode.name}");
		}

		switch (_destinationType)
		{
			case CrewActivityType.Restore:
				FindAvailableNode(CrewActivityType.Restore);
				break;
			case CrewActivityType.Relax:
				FindAvailableNode(CrewActivityType.Relax);
				break;
			case CrewActivityType.Train:
				FindAvailableNode(CrewActivityType.Train);
				break;
			case CrewActivityType.Work:
				FindAvailableNode(CrewActivityType.Work);
				break;
			default:
				FindAvailableNode(CrewActivityType.Idle);
				break;
		}
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
			if (needKey.PositionNodeType == _assignedPositionNode.NodeType && TargetIsReached())
			{
				newValue += _baseReplenishRate * needKey.ReplenishmentModifier;
			}
			else
			{
				newValue -= _baseDecayRate * needKey.DecayModifier;
			}
			_needs[needKey] = newValue;
			//Debug.Log($"{this.name} {needKey.name} = {_needs[needKey]}");
		}
	}
}
