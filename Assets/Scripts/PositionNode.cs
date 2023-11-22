using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionNode : MonoBehaviour, INeedResolver
{
    [SerializeField]
    private Need _needSatisfied;
    [SerializeField]
    private bool _isAvailable = true;
    [SerializeField]
    private PositionNodeSet _positionNodeSet;

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

    public void ResolveNeed(GameObject needyObject)
    {
        Needs needs = needyObject.GetComponent<Needs>();
        // access RestoreNeed method for need.
    }
}
