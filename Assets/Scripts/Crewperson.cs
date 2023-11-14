using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Crewperson : MonoBehaviour
{
	[SerializeField]
	private float _intellectualNeeds = 1.0f;
	[SerializeField]
	private float _emotionalNeeds = 1.0f;
	[SerializeField]
	private float _physicalNeeds = 1.0f;
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

		if (_physicalNeeds < _needsTolerance)
		{
			_currentPriority = PositionNodeType.Restore;
			return;
		}

		if (_emotionalNeeds < _needsTolerance)
		{
			_currentPriority = PositionNodeType.Relax;
			return;
		}

		if (_intellectualNeeds < _needsTolerance)
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
				_canMove = _intellectualNeeds;
				break;
			case PositionNodeType.Relax:
				_canMove = _emotionalNeeds;
				break;
			case PositionNodeType.Restore:
				_canMove = _physicalNeeds;
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
					DecayPhysical(_baseDecayRate);
					DecayEmotional(_baseDecayRate);
					ReplenishIntellectual(_baseReplenishRate);
					break;
				case PositionNodeType.Relax:
					Debug.Log("Relaxing...");
					DecayPhysical(_baseDecayRate);
					ReplenishEmotional(_baseReplenishRate);
					DecayIntellectual(_baseDecayRate);
					break;
				case PositionNodeType.Restore:
					Debug.Log("Sleeping...");
					ReplenishPhysical(_baseReplenishRate);
					DecayEmotional(_baseDecayRate);
					DecayIntellectual(_baseDecayRate);
					break;
				case PositionNodeType.Work:
					Debug.Log("Working...");
					DecayPhysical(_baseDecayRate * 0.6f);
					DecayEmotional(_baseDecayRate * 0.6f);
					DecayIntellectual(_baseDecayRate * 0.6f);
					break;
				default:
					Debug.Log("Idling...");
					DecayPhysical(_baseDecayRate);
					DecayEmotional(_baseDecayRate);
					DecayIntellectual(_baseDecayRate);
					break;
			}
		} 
		else
		{
			Debug.Log("Not close enough to a position node");
			DecayPhysical(_baseDecayRate);
			DecayEmotional(_baseDecayRate);
			DecayIntellectual(_baseDecayRate);
		}
	}

	private void DecayPhysical(float decayRate)
	{
		_physicalNeeds -= decayRate;
		_physicalNeeds = Mathf.Clamp(_physicalNeeds, 0.0f, 1.0f);
	}

	private void DecayEmotional(float decayRate)
	{
		_emotionalNeeds -= decayRate;
		_emotionalNeeds = Mathf.Clamp(_emotionalNeeds, 0.0f, 1.0f);
	}

	private void DecayIntellectual(float decayRate)
	{
		_intellectualNeeds -= decayRate;
		_intellectualNeeds = Mathf.Clamp(_intellectualNeeds, 0.0f, 1.0f);
	}

	private void ReplenishPhysical(float replenishRate)
	{
		_physicalNeeds += replenishRate;
		_physicalNeeds = Mathf.Clamp(_physicalNeeds, 0.0f, 1.0f);
	}

	private void ReplenishEmotional(float replenishRate)
	{
		_emotionalNeeds += replenishRate;
		_emotionalNeeds = Mathf.Clamp(_emotionalNeeds, 0.0f, 1.0f);
	}

	private void ReplenishIntellectual(float replenishRate)
	{
		_intellectualNeeds += replenishRate;
		_intellectualNeeds = Mathf.Clamp(_intellectualNeeds, 0.0f, 1.0f);
	}
}
