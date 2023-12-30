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
    public StateVariable Variable => _stateVariable;
    public StateCondition Condition 
    { 
        get 
        {
            float value = Mathf.Floor(_stateVariable.Value / 0.25f + 0.5f) * 0.25f;
            value = value * 100;
            if (_stateVariable.Value < 0) value *= -1;

            return (StateCondition)(value); 
        } 
    }



}
