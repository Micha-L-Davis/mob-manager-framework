using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrewPositionNode : MonoBehaviour
{
    [SerializeField]
    private CrewNeed _needSatisfied;
    [SerializeField]
    private bool _isAvailable = true;
    [SerializeField]
    private PositionNodeSet _positionNodeSet;

    public CrewNeed NeedSatisfied { get { return _needSatisfied; } }
    //public CrewActivityType NodeType { get { return _nodeType; } }
    public bool IsAvailable { get { return _isAvailable; }  set { _isAvailable = value; } }

    private void Awake()
    {
        //join runtime set
        _positionNodeSet.Items.Add(this);
    }

    private void OnDisable()
    {
        _positionNodeSet.Items.Remove(this);
    }

    private void OnDestroy()
    {
        //leave runtime set
        _positionNodeSet.Items.Remove(this);
    }
}
