using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StateNotification
{
    StateVariable _state;
    public StateVariable State => _state;
    StatefulObject _notifier;
    public StatefulObject Notifier => _notifier;

    public StateNotification(StateVariable state, StatefulObject notifier)
    {
        _state = state;
        _notifier = notifier;
    }
}
