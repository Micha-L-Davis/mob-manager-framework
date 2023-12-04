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
    public float PriorityNeedValue 
    { 
        get 
        {
            if (_priorityNeed == null) return 1.0f;

            return _needs[_priorityNeed]; 
        } 
        private set 
        { 
            _needs[_priorityNeed] = value; 
        } 
    }

    [SerializeField]
    private ResolverSet _needResolverList;
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
        if (PriorityNeedValue >= 1.0f - _priorityNeed?.NeedTolerance && ResolverInRange())
        {
            _priorityNeed = CalculatePriorities();
            FindAvailableResolver(_priorityNeed);
            return;
        }

        // double check priorities, in case we don't know what to do.
        var newNeed = CalculatePriorities();
        if (newNeed != _priorityNeed)
        {
            _priorityNeed = newNeed;
            FindAvailableResolver(_priorityNeed);
        }
    }

    private void ProcessNeedsChange()
    {
        List<Need> resolvableNeeds = new List<Need>();
        System.Func<Needs, bool> resolve = (_) => false;
        if (_assignedResolver != null)
        {
            resolvableNeeds = _assignedResolver.ResolvableNeeds;
            resolve = _assignedResolver.ResolveNeed;
        }

        List<Need> keys = new List<Need>(_needs.Keys);
        foreach (Need needKey in keys)
        {
            float newValue = _needs[needKey];
            if (resolvableNeeds.Contains(needKey))
            {
                bool isResolved = resolve(this);
                Debug.Log($"{name}'s need for {needKey} is resolved? {isResolved}");
                if (isResolved) //don't decay
                    continue;
            }

            newValue -= _baseDecayRate * needKey.DecayModifier;

            newValue = Mathf.Clamp(newValue, 0.0f, 1.0f);
            Debug.Log($"{this.name} {needKey.name} = {newValue}");
            _needs[needKey] = newValue;

            // Here we need to trigger what happens if the need value is at 0.

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
        INeedResolver needResolver = _needResolverList.Items.Where(node => node.ResolvableNeeds.Contains(type) && node.IsAvailable).FirstOrDefault();
        if (needResolver == null)
        {
            Debug.LogWarning($"{name} cannot find available resolver for {type}!");
            return;
        }

        if (needResolver.ResolvableNeeds == _assignedResolver?.ResolvableNeeds) return;

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
        _needs.Add(newNeed, value);
        _priorityNeed = CalculatePriorities();
    }

    public void ReplenishNeed(Need need)
    {
        if (_needs.ContainsKey(need))
        {
            _needs[need] += _baseReplenishRate * need.ReplenishmentModifier;
        }
    }
}
