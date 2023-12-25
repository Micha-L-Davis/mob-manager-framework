using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestorationPoint : MonoBehaviour, INeedResolver
{
    [SerializeField]
    private List<Need> _needsSatisfied;
    [SerializeField]
    private bool _isAvailable = true;
    [SerializeField]
    private ResolverSet _restorationSet;
    [SerializeField]
    private ResolverSet _resolverSet;

    public List<Need> ResolvableNeeds { get { return _needsSatisfied; } }

    public bool IsAvailable { get { return _isAvailable; }  set { _isAvailable = value; } }

    public Transform Transform => this.gameObject.transform;

    private void OnEnable()
    {
        //join runtime set
        _restorationSet.Items.Add(this);
        _resolverSet.Items.Add(this);
    }

    private void OnDisable()
    {
        //leave runtime set
        _restorationSet.Items.Remove(this);
        _resolverSet.Items.Remove(this);
    }

    public bool ProvideResolution(Needs needyObject)
    {
        if (Vector3.Distance(this.transform.position, needyObject.transform.position) < 0.5f)
        {
            foreach (Need need in ResolvableNeeds)
            {
                needyObject.ReplenishNeed(need);
            }
            return true;
        }
        return false;
    }
}
