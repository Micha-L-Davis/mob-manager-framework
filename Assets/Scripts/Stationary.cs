using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stationary : MonoBehaviour, INeedResolver
{
    [SerializeField]
    private Need _needSatisfied;
    [SerializeField]
    private bool _isAvailable = true;
    [SerializeField]
    private StationarySet _positionNodeSet;

    public Need ResolvableNeed { get { return _needSatisfied; } }

    public bool IsAvailable { get { return _isAvailable; }  set { _isAvailable = value; } }

    public Transform Transform => this.gameObject.transform;

    private void Awake()
    {
        //join runtime set
        _positionNodeSet.Items.Add(this);
    }

    private void OnDestroy()
    {
        //leave runtime set
        _positionNodeSet.Items.Remove(this);
    }

    public bool ResolveNeed(Needs needyObject)
    {
        if (Vector3.Distance(this.transform.position, needyObject.transform.position) > 0.5f)
        {
            needyObject.ReplenishNeed(ResolvableNeed);
            return true;
        }
        return false;
    }
}
