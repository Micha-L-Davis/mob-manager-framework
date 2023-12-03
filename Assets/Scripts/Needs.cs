using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Needs : MonoBehaviour
{
    private Dictionary<Need, float> _needs = new Dictionary<Need, float>();
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
    private Need _priorityNeed;
    public Need PriorityNeed { get { return _priorityNeed; } }
    public float PriorityNeedValue { get { return _needs[_priorityNeed]; } private set { _needs[_priorityNeed] = value; } }

    [SerializeField]
    private RuntimeSet<INeedResolver> _needResolverList;
    [SerializeField]
    private INeedResolver _assignedResolver;
    public INeedResolver AssignedResolver => _assignedResolver;
    [SerializeField]
    private float _resolverRangeTolerance = 0.5f;

    private void Awake()
    {
        // Initialize the needs dictionary
        foreach (Need need in _needsList.Items)
        {
            _needs.Add(need, _initialNeedValue);
        }

        _priorityNeed = CalculatePriorities();
    }

    void Start()
    {
        if (_priorityNeed != null)
        {
            PriorityNeedValue = 1.0f - _priorityNeed.NeedTolerance;
            FindAvailableResolver(_priorityNeed);
        }
    }

    void Update()
    {
        if (Time.time > _needsChangeTimer)
        {
            _needsChangeTimer = Time.time + _needsChangeCooldown;
            ProcessNeedsChange();
        }

        // if the current priority is topped off AND we're next to the resolver, we are now available.
        if (PriorityNeedValue >= 1.0f - _priorityNeed.NeedTolerance && ResolverInRange())
        {
            _priorityNeed = CalculatePriorities();
            FindAvailableResolver(_priorityNeed);
        }
    }

    // refactor below. Instead we need a method to modify needs based on input from resolver.
    // Let the resolver deside how the need is resolved.  Needs class should not be concerned with why needs change.
    private void ProcessNeedsChange()
    {
        float distance = Vector3.Distance(transform.position, _assignedResolver.Transform.position);

        List<Need> keys = new List<Need>(_needs.Keys);
        foreach (Need needKey in keys)
        {
            float newValue = _needs[needKey];
            if (needKey == _assignedResolver.ResolvableNeed)
            {
                bool isResolved = _assignedResolver.ResolveNeed(this);

                if (isResolved) //don't decay
                    continue;
            }

            newValue -= _baseDecayRate * needKey.DecayModifier;

            newValue = Mathf.Clamp(newValue, 0.0f, 1.0f);
            _needs[needKey] = newValue;
            //Debug.Log($"{this.name} {needKey.name} = {_needs[needKey]}");
        }
    }

    public bool ResolverInRange()
    {
        //Debug.Log($"{this.name} is resolved by {_assignedResolver?.Transform.name}");
        float distance = Vector3.Distance(transform.position, _assignedResolver.Transform.position);
        if (distance >= _resolverRangeTolerance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private Need CalculatePriorities()
    {
        foreach (KeyValuePair<Need, float> need in _needs.OrderBy(n => n.Key.Priority))
        {
            //Debug.Log($"Checking {need.Key.name} at {need.Key.Priority}. Is {need.Value} < {need.Key.NeedTolerance}?");
            if (need.Value < need.Key.NeedTolerance)
            {
                return need.Key;
            }

            
        }

        return _priorityNeed;
    }

    void FindAvailableResolver(Need type)
    {
        INeedResolver needResolver = _needResolverList.Items.Where(node => node.ResolvableNeed == type && node.IsAvailable).FirstOrDefault();
        if (needResolver == null)
        {
            //Debug.LogWarning("No available node found!");
            return;
        }

        if (needResolver.ResolvableNeed == _assignedResolver?.ResolvableNeed) return;

        needResolver.IsAvailable = false;
        if (_assignedResolver != null)
        {
            _assignedResolver.IsAvailable = true;
        }
        _assignedResolver = needResolver;
        //Debug.Log($"{gameObject.name} assigned to {_assignedPositionNode.name}");
    }

    public void AddNeed(Need newNeed, float value)
    {
        _needsList.Items.Add(newNeed);
        _needs.Add(newNeed, value);
    }

    public void ReplenishNeed(Need need)
    {
        if (_needs.ContainsKey(need))
        {
            _needs[need] += _baseReplenishRate * need.ReplenishmentModifier;
        }
    }
}
