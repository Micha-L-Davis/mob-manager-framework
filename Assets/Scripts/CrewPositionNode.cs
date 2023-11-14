using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PositionNodeType
{
    Work,
    Restore,
    Relax,
    Idle
}

public class CrewPositionNode : MonoBehaviour
{


    [SerializeField]
    private PositionNodeType _nodeType;
    [SerializeField]
    private bool _isAvailable = true;

    public PositionNodeType NodeType { get { return _nodeType; } }
    public bool IsAvailable { get { return _isAvailable; }  set { _isAvailable = value; } }

}
