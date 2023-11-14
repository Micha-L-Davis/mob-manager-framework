using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionNodeType
{
    Restore,
    Relax,
    Train,
    Work,
    Idle
}

public class CrewPositionNode : MonoBehaviour
{
    [SerializeField]
    private PositionNodeType _nodeType;
    [SerializeField]
    private bool _isAvailable = true;
    [SerializeField]
    private PositionNodeSet _positionNodeSet;

    public PositionNodeType NodeType { get { return _nodeType; } }
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
