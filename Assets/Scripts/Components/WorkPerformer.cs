using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;

[RequireComponent(typeof(StateResponder))]
public class WorkPerformer : MonoBehaviour
{
    [SerializeField]
    List<State> _responsibilities = new List<State>(); // Index in priority order, 0 = highest priority
    //[SerializeField]
    //WorkSlotSet _workPositions;
    [SerializeField]
    State _activeResponsibility;
    float _baseLabor = 0.5f;

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
        _responderComponent = GetComponent<StateResponder>();

        InitializeEventListeners();
    }

    private void InitializeEventListeners()
    {
        StateChangeEventListener[] listeners = gameObject.GetComponents<StateChangeEventListener>();
        for (int i = 0; i < _responsibilities.Count; i++)
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

    private void EvaluateResponsibility(StateNotification stateNotification)
    {
        // apply incoming state condition to noteScore

        // apply distance to target object to noteScore

        // apply active responsibility condition to activeScore

        // apply distance to active object to activeScore

        // apply weight to activeScore (for stickiness)

        // compare scores, switch activeResponsibility if necessary

        // TendResponsibility if it has changed

        if (_activeResponsibility.Variable != stateNotification.State.Variable)
        {
            TendResponsibility(stateNotification);
        }
    }

    private void TendResponsibility(StateNotification stateNotification)
    {
        // find a work position on the target object
        WorkCenter workCenter = stateNotification.Notifier.GetComponent<WorkCenter>();
        if (stateNotification.Notifier == null) return;
        WorkSlot slot = workCenter.GetAvailableSlot();
        // if distance is too great, signal the responder component
        if (Vector3.Distance(this.transform.position, stateNotification.Notifier.transform.position) > 0.25f)
        {
            _responderComponent.Respond(stateNotification.Notifier.gameObject);
        }
        // if distance is near, apply labor to the work position
        else
        {
            slot.ApplyLabor(stateNotification.State.Variable, _baseLabor);
        }
    }


}
