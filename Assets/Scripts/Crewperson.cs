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
    private float _needsTolerance = 0.25f;

    [SerializeField]
    private float _decayRate = 0.01f;
    [SerializeField]
    private float _replenishRate = 0.1f;
    [SerializeField]
    private float _needsChangeCooldown = 1.0f;
    private float _needsChangeTimer = -1.0f;
    private float _canMove = -1.0f;

    [SerializeField]
    private CrewPositionNode _assignedPositionNode;
    [SerializeField]
    private List<CrewPositionNode> _crewPositionNodes;
    [SerializeField]
    private NavMeshAgent _navMeshAgent;
    [SerializeField]
    private PositionNodeType _currentPriority;

    void Start()
    {
        _canMove = 1.0f - _needsTolerance;
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
            _currentPriority = PositionNodeType.Work;
            return;
        }

        _currentPriority = PositionNodeType.Idle;
    }

    private void CalculateCanMove()
    {
        // Maintain the _canMove variable so that the crewperson remains on station until their need is met
        switch (_assignedPositionNode.NodeType)
        {
            case PositionNodeType.Work:
                _canMove = _intellectualNeeds;
                break;
            case PositionNodeType.Relax:
                _canMove = _emotionalNeeds;
                break;
            case PositionNodeType.Restore:
                _canMove = _physicalNeeds;
                break;
            default:
                _canMove = 1.0f - _needsTolerance; // No needs are served here, we should move on quickly
                break;
        }
    }

    private bool TargetIsReached()
    {
        float distance = Vector3.Distance(transform.position, _assignedPositionNode.transform.position);
        if (distance >= 0.5f)
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
            CrewPositionNode positionNode = _crewPositionNodes.Where(node => node.NodeType == type && node.IsAvailable).FirstOrDefault();
            if (positionNode == null)
            {
                Debug.LogWarning("No available node found!");
                return;
            }

            positionNode.IsAvailable = false;
            _assignedPositionNode.IsAvailable = true;
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
        _navMeshAgent.destination = _assignedPositionNode.transform.position;
    }

    private void ProcessNeedsChange()
    {
        float distance = Vector3.Distance(transform.position, _assignedPositionNode.transform.position);

        if (TargetIsReached())
        {
            switch (_assignedPositionNode.NodeType)
            {
                case PositionNodeType.Work:
                    Debug.Log("Working...");
                    DecayPhysical();
                    DecayEmotional();
                    ReplenishIntellectual();
                    break;
                case PositionNodeType.Relax:
                    Debug.Log("Relaxing...");
                    DecayPhysical();
                    ReplenishEmotional();
                    DecayIntellectual();
                    break;
                case PositionNodeType.Restore:
                    Debug.Log("Sleeping...");
                    ReplenishPhysical();
                    DecayEmotional();
                    DecayIntellectual();
                    break;
                default:
                    Debug.Log("Idling...");
                    DecayPhysical();
                    DecayEmotional();
                    DecayIntellectual();
                    break;
            }
        } 
        else
        {
            Debug.Log("Not close enough to a position node");
            DecayPhysical();
            DecayEmotional();
            DecayIntellectual();
        }
    }

    private void DecayPhysical()
    {
        _physicalNeeds -= _decayRate;
        _physicalNeeds = Mathf.Clamp(_physicalNeeds, 0.0f, 1.0f);
    }

    private void DecayEmotional()
    {
        _emotionalNeeds -= _decayRate;
        _emotionalNeeds = Mathf.Clamp(_emotionalNeeds, 0.0f, 1.0f);
    }

    private void DecayIntellectual()
    {
        _intellectualNeeds -= _decayRate;
        _intellectualNeeds = Mathf.Clamp(_intellectualNeeds, 0.0f, 1.0f);
    }

    private void ReplenishPhysical()
    {
        _physicalNeeds += _replenishRate;
        _physicalNeeds = Mathf.Clamp(_physicalNeeds, 0.0f, 1.0f);
    }

    private void ReplenishEmotional()
    {
        _emotionalNeeds += _replenishRate;
        _emotionalNeeds = Mathf.Clamp(_emotionalNeeds, 0.0f, 1.0f);
    }

    private void ReplenishIntellectual()
    {
        _intellectualNeeds += _replenishRate;
        _intellectualNeeds = Mathf.Clamp(_intellectualNeeds, 0.0f, 1.0f);
    }
}
