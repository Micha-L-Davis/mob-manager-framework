using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crewperson : MonoBehaviour
{
	[SerializeField]
	private Need _intellectualNeeds;
	[SerializeField]
	private Need _emotionalNeeds;
	[SerializeField]
	private Need _physicalNeeds;
	[SerializeField]
	private List<CrewPositionNode> _duties = new List<CrewPositionNode>();

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
	private float _canMove = -1.0f;

	[SerializeField]
	private CrewPositionNode _assignedPositionNode;
	[SerializeField]
	private PositionNodeSet _crewPositionNodes;
	[SerializeField]
	private NavMeshAgent _navMeshAgent;
	[SerializeField]
	private PositionNodeType _currentPriority;

	void Start()
	{
		_canMove = 1.0f - _needsTolerance;
		AssignPositionNode();
		SetDestination();
	}

	void Update()
	{
		CalculatePriorities();
		CalculateCanMove();

		if (_canMove >= 1.0f - _needsTolerance && TargetIsReached())
		{
			AssignPositionNode();
			SetDestination();
		}

	
		if (Time.time > _needsChangeTimer)
		{
			_needsChangeTimer = Time.time + _needsChangeCooldown;
			ProcessNeedsChange();
		}
	}

	private void CalculatePriorities()
	{

		if (_physicalNeeds.Value < _needsTolerance)
		{
			_currentPriority = PositionNodeType.Restore;
			return;
		}

		if (_emotionalNeeds.Value < _needsTolerance)
		{
			_currentPriority = PositionNodeType.Relax;
			return;
		}

		if (_intellectualNeeds.Value < _needsTolerance)
		{
			_currentPriority = PositionNodeType.Train;
			return;
		}

		_currentPriority = PositionNodeType.Idle;
	}

	private void CalculateCanMove()
	{
		// Maintain the _canMove variable so that the crewperson remains on station until their need is met
		switch (_assignedPositionNode.NodeType)
		{
			case PositionNodeType.Train:
				_canMove = _intellectualNeeds.Value;
				break;
			case PositionNodeType.Relax:
				_canMove = _emotionalNeeds.Value;
				break;
			case PositionNodeType.Restore:
				_canMove = _physicalNeeds.Value;
				break;
			case PositionNodeType.Work:
				if (_duties.Count == 0)
				{
					_canMove = 1.0f - _needsTolerance;
				}
				break;
			default:
				_canMove = 1.0f - _needsTolerance; // No needs are served here, we should move on quickly
				break;
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
		void FindAvailableNode(PositionNodeType type)
		{
			CrewPositionNode positionNode = _crewPositionNodes.Items.Where(node => node.NodeType == type && node.IsAvailable).FirstOrDefault();
			if (positionNode == null)
			{
				Debug.LogWarning("No available node found!");
				return;
			}

			if (positionNode.NodeType == _assignedPositionNode?.NodeType) return;

			positionNode.IsAvailable = false;
			if (_assignedPositionNode != null)
            {
				_assignedPositionNode.IsAvailable = true;
            }
			_assignedPositionNode = positionNode;
			Debug.Log($"{gameObject.name} assigned to {_assignedPositionNode.name}");
		}

		switch (_currentPriority)
		{
			case PositionNodeType.Restore:
				FindAvailableNode(PositionNodeType.Restore);
				break;
			case PositionNodeType.Relax:
				FindAvailableNode(PositionNodeType.Relax);
				break;
			case PositionNodeType.Train:
				FindAvailableNode(PositionNodeType.Train);
				break;
			case PositionNodeType.Work:
				FindAvailableNode(PositionNodeType.Work);
				break;
			default:
				FindAvailableNode(PositionNodeType.Idle);
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

		if (TargetIsReached())
		{
			switch (_assignedPositionNode.NodeType)
			{
				case PositionNodeType.Train:
					Debug.Log("Training...");
					ModifyNeed(ref _physicalNeeds, -_baseDecayRate * 0.6f);
					ModifyNeed(ref _emotionalNeeds, -_baseDecayRate * 0.6f);
					ModifyNeed(ref _intellectualNeeds, _baseReplenishRate);
					break;
				case PositionNodeType.Relax:
					Debug.Log("Relaxing...");
					ModifyNeed(ref _physicalNeeds, -_baseDecayRate);
					ModifyNeed(ref _emotionalNeeds, _baseReplenishRate);
					ModifyNeed(ref _intellectualNeeds, -_baseDecayRate * 0.6f);
					break;
				case PositionNodeType.Restore:
					Debug.Log("Sleeping...");
					ModifyNeed(ref _physicalNeeds, _baseReplenishRate);
					ModifyNeed(ref _emotionalNeeds, -_baseDecayRate * 0.6f);
					ModifyNeed(ref _intellectualNeeds, -_baseDecayRate * 0.6f);
					break;
				case PositionNodeType.Work:
					Debug.Log("Working...");
					ModifyNeed(ref _physicalNeeds, -_baseDecayRate * 0.6f);
					ModifyNeed(ref _emotionalNeeds, -_baseDecayRate * 0.6f);
					ModifyNeed(ref _intellectualNeeds, -_baseDecayRate * 0.6f);
					break;
				default:
					Debug.Log("Idling...");
					ModifyNeed(ref _physicalNeeds, -_baseDecayRate);
					ModifyNeed(ref _emotionalNeeds, -_baseDecayRate);
					ModifyNeed(ref _intellectualNeeds, -_baseDecayRate);
					break;
			}
		} 
		else
		{
			Debug.Log("Not close enough to a position node");
			ModifyNeed(ref _physicalNeeds, -_baseDecayRate);
			ModifyNeed(ref _emotionalNeeds, -_baseDecayRate);
			ModifyNeed(ref _intellectualNeeds, -_baseDecayRate);
		}
	}

	private void ModifyNeed(ref Need need, float amount)
	{
		need.Value += amount;
	}
}
