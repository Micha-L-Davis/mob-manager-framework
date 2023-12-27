using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;

public class WorkPerformer : MonoBehaviour
{
    [SerializeField]
    List<State> _responsibilities = new List<State>();
    [SerializeField]
    WorkSlotSet _workPositions;

    StateResponder _responderComponent;
    private void Awake()
    {
        // create Unity Event components for each responsibilty
        foreach (State state in _responsibilities)
        {
            gameObject.AddComponent<StateChangeEventListener>();
        }
    }
    private void Start()
    {
        StateChangeEventListener[] listeners = gameObject.GetComponents<StateChangeEventListener>();
        for (int i = 0; i < _responsibilities.Count;i++)
        {
            StateChangeEvent gameEvent = _responsibilities[i].OnStateChange;
            StateChangeEventListener listener = listeners[i];
            listener.enabled = false;
            listener.Event = gameEvent;

            System.Type[] types = new[] { typeof(StateNotification) };
            System.Reflection.MethodInfo targetInfo = UnityEvent.GetValidMethodInfo(this, nameof(EvaluateResponsibility), types);
            UnityAction<StateNotification> methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<StateNotification>), this, targetInfo, true) as UnityAction<StateNotification>;
            UnityEventTools.AddPersistentListener(listener.Response, methodDelegate);

            listener.enabled = true;
        }
    }

    public void EvaluateResponsibility(StateNotification stateNotification)
    {
        // check the values of the incoming state change against internal priorities and tolerances

    }

    private void TendResponsibility(StateNotification stateNotification)
    {
        // manipulate the state value and do whatever makes sense here.
    }
}
