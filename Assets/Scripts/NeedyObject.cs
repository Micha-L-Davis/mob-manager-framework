using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Needs))]
public abstract class NeedyObject : MonoBehaviour, INeedResolver
{
    [SerializeField]
    internal ResolverSet _resolverSet;
    [SerializeField]
    internal List<Need> _resolvableNeeds;
    internal Needs _needs;
    public List<Need> ResolvableNeeds => _resolvableNeeds;
    public Transform Transform => gameObject.transform;
    [SerializeField]
    private bool _isAvailable = true;
    public bool IsAvailable { get => _isAvailable; set => _isAvailable = value; }

    public abstract bool ProvideResolution(Needs needyObject);
    public abstract void SeekResolution(INeedResolver resolver);

    internal virtual void OnEnable()
    {
        Debug.Log($"{this.name} is joining {_resolverSet.name}");
        _resolverSet.Items.Add(this);
    }

    internal virtual void Start()
    {
        _needs = GetComponent<Needs>();
    }

    internal virtual void OnDisable()
    {
        _resolverSet.Items.Remove(this);
    }
}
