using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StateNotification
{
    State _state;
    public State State => _state;
    StatefulObject _notifier;
    public StatefulObject Notifier => _notifier;

    public StateNotification(State state, StatefulObject notifier)
    {
        _state = state;
        _notifier = notifier;
    }
}
