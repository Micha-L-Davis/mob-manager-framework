using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Asset/Crew Need")]
public class CrewNeed : ScriptableObject
{
    [SerializeField]
    private int _priority;
    public int Priority { get { return _priority; } set { _priority = value; } }

    //[SerializeField]
    //private CrewActivityType _positionNodeType;
    //public CrewActivityType PositionNodeType { get { return _positionNodeType; } set { _positionNodeType = value; } }

    [SerializeField]
    private ProficiencyType _requiredProficiency;
    public ProficiencyType RequiredProficiency { get { return _requiredProficiency; } }

    [SerializeField]
    private float _needTolerance = 0.25f;
    public float NeedTolerance { get { return _needTolerance;} }

    [SerializeField]
    private float _replenishmentModifier = 1.0f;
    public float ReplenishmentModifier { get { return _replenishmentModifier; } }
    [SerializeField]
    private float _decayModifier = 1.0f;
    public float DecayModifier { get { return _decayModifier; } }
}
