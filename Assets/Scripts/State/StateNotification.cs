using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct StateNotification
{
    FloatVariable _state;
    public FloatVariable State => _state;
    StatefulObject _notifier;
    public StatefulObject Notifier => _notifier;

}
