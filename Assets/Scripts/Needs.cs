using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Needs : MonoBehaviour
{
    private Dictionary<Need, float> _needs = new Dictionary<Need, float>();
    public Dictionary<Need, float> NeedsDictionary => _needs;
    [SerializeField]
    internal NeedsList _needsList;
    internal NeedyObject _needyObject;

    [SerializeField]
    internal float _initialNeedValue = 0.5f;

    [SerializeField]
    private float _baseDecayRate = 0.01f;
    [SerializeField]
    private float _baseReplenishRate = 0.1f;

    [SerializeField]
    private float _needsChangeCooldown = 1.0f;
    private float _needsChangeTimer = -1.0f;

    internal Need _priorityNeed;
    [SerializeField]
    private Need _defaultNeed;
    public Need PriorityNeed 
    { 
        get 
        { 
            if (_priorityNeed != null) return _priorityNeed; 
            return _defaultNeed;
        } 
    }
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
    private List<ResolverSet> _needResolverLists = new List<ResolverSet>();
    [SerializeField]
    internal INeedResolver _assignedResolver;
    public INeedResolver AssignedResolver => _assignedResolver;
    [SerializeField]
    private float _resolverRangeTolerance = 0.5f;

    internal void Awake()
    {
        // Initialize the needs dictionary
        foreach (Need need in _needsList.Items)
        {
            _needs.Add(need, _initialNeedValue);
        }

        _priorityNeed = CalculatePriorities();

        _needyObject = gameObject.GetComponent<NeedyObject>();
        if(_needyObject == null)
        {
            Debug.LogError("No NeedyObject found.  A Needs component must be attached to some type of NeedyObject!");
        }
    }

    void Start()
    {
        if (_priorityNeed != null)
        {
            PriorityNeedValue = 1.0f - _priorityNeed.NeedTolerance;
            ResolveNeeds();
        }
    }

    void Update()
    {
        if (Time.time > _needsChangeTimer)
        {
            _needsChangeTimer = Time.time + _needsChangeCooldown;
            ProcessNeedsChange();
        }

        Need newNeed;

        // if the current priority is topped off AND we're next to the resolver, we are now available.
        if (PriorityNeedValue >= 1.0f - _priorityNeed?.NeedTolerance && ResolverInRange())
        {
            Debug.Log("current priority is topped off AND we're next to the resolver; we are now available.");
            newNeed = CalculatePriorities();
            Debug.Log(newNeed);
            if (newNeed != _priorityNeed)
            {
                _priorityNeed = newNeed;
                ResolveNeeds();
            }
            return;
        }

        //// double check priorities, in case we don't know what to do.
        //Debug.Log("Double checking priorities, since we might not know what to do at this point.");
        //newNeed = CalculatePriorities();
        //if (newNeed != _priorityNeed)
        //{
        //    _priorityNeed = newNeed;
        //    ResolveNeeds();
        //}
    }

    internal void ProcessNeedsChange()
    {
        List<Need> resolvableNeeds = new List<Need>();
        System.Func<Needs, bool> resolve = (_) => false;
        if (_assignedResolver != null)
        {
            resolvableNeeds = _assignedResolver.ResolvableNeeds;
            resolve = _assignedResolver.ProvideResolution;
        }

        List<Need> keys = new List<Need>(_needs.Keys);
        foreach (Need needKey in keys)
        {
            float newValue = _needs[needKey];
            if (resolvableNeeds.Contains(needKey))
            {
                bool isResolved = resolve(this);
                //Debug.Log($"{name}'s need for {needKey} is resolved? {isResolved}");
                if (isResolved) //don't decay
                    continue;
            }

            newValue -= _baseDecayRate * needKey.DecayModifier;

            newValue = Mathf.Clamp(newValue, 0.0f, 1.0f);
            _needs[needKey] = newValue;

            // Here we need to trigger what happens if the need value is at 0.

        }

    }

    internal void ResolveNeeds()
    {
        INeedResolver needResolver = FindAvailableResolver(_priorityNeed);
        if (needResolver == null) return;

        needResolver.IsAvailable = false;
        if (_assignedResolver != null)
        {
            _assignedResolver.IsAvailable = true;
        }
        _assignedResolver = needResolver;
        Debug.Log($"{_assignedResolver} is assigned to {_needyObject.name}");
        _needyObject.SeekResolution(_assignedResolver);
    }

    internal bool ResolverInRange()
    {
        if (_assignedResolver == null) return true;
        Debug.Log($"{this.name} can be resolved by {_assignedResolver?.Transform.name}");

        float distance = Vector3.Distance(transform.position, _assignedResolver.Transform.position);
        if (distance > _resolverRangeTolerance)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    internal Need CalculatePriorities()
    {
        foreach (KeyValuePair<Need, float> need in _needs.OrderBy(n => n.Key.Priority))
        {
            if (need.Value < need.Key.NeedTolerance)
            {
                return need.Key;
            }
        }
        return _defaultNeed;
    }

    internal INeedResolver FindAvailableResolver(Need needToResolve)
    {
        INeedResolver needResolver = null;
        foreach(ResolverSet needResolverList in _needResolverLists)
        {
            Debug.Log($"Checking if {needResolverList.Items[0].Transform.name} can resolve {needToResolve}");
            needResolver = needResolverList.Items.Where(item => item.ResolvableNeeds.Contains(needToResolve) && item.IsAvailable).FirstOrDefault();
            if (needResolver == null)
            {
                Debug.LogWarning($"{name} cannot find available resolver for {needToResolve}!");
                continue;
            }
        }

        if (needResolver == null) return null;

        int target = needResolver.ResolvableNeeds.Count;
        int found = 0;
        foreach (Need need in needResolver.ResolvableNeeds)
        {
            if (_assignedResolver != null && _assignedResolver.ResolvableNeeds.Contains(need))
            {
                found++;
            }
        }
        if (target == found) return null;
        
        return needResolver;
    }

    public void AddNeed(Need newNeed, float value)
    {
        Debug.Log($"Adding {newNeed}:{value} to {this.name}");
        if (!_needs.ContainsKey(newNeed))
        {
            _needs.Add(newNeed, value);
        }
        else
        {
            _needs[newNeed] = value;
        }

        newNeed = CalculatePriorities();
        Debug.Log($"{newNeed}, {_priorityNeed}");
        if (newNeed != _priorityNeed)
        {
            _priorityNeed = newNeed;
            ResolveNeeds();
        }
    }

    public void ReplenishNeed(Need need)
    {
        if (_needs.ContainsKey(need))
        {
            _needs[need] += _baseReplenishRate * need.ReplenishmentModifier;
        }
    }
}
