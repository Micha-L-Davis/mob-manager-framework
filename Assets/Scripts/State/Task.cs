using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Task
{
    [SerializeField]
    State _subjectState;
    public State State => _subjectState;
    [SerializeField]
    StatefulObject _targetObject;
    public StatefulObject TargetObject => _targetObject;
    [SerializeField]
    int _priority;
    public int Priority => _priority;
    [SerializeField]
    StateCondition _completionCondition = StateCondition.Nominal;
    public StateCondition CompletionCondition => _completionCondition;

    public Task(StatefulObject notifier, State state)
    {
        _subjectState = state;
        _targetObject = notifier;
        _priority = state.Priority;
    }

    public void SetPriority(int value)
    {
        _priority = value;
    }
}
