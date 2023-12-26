using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct State
{
    [SerializeField]
    public StateChangeEvent OnStateChange;
    [SerializeField]
    FloatVariable _stateVariable;
    public FloatVariable StateVariable => _stateVariable;

    public State(StateChangeEvent eventAsset, FloatVariable variableAsset)
    {
        OnStateChange = eventAsset;
        _stateVariable = variableAsset;
    }
}
