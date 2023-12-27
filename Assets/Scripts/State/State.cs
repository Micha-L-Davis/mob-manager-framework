using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct State
{
    [SerializeField]
    public StateChangeEvent OnStateChange;
    [SerializeField]
    StateVariable _stateVariable;
    public StateVariable StateVariable => _stateVariable;

    [SerializeField]
    private float _upperTolerance;
    [SerializeField]
    private float _lowerTolerance;


}
