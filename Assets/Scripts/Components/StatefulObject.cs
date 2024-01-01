using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatefulObject : MonoBehaviour
{
    [SerializeField]
    List<State> _states = new List<State>();
    [SerializeField]
    TaskQueue _workQueue;
    public TaskQueue WorkQueue => _workQueue;


    private void Update()
    {
        if (_states.Count == 0) return;

        for (int i = 0; i < _states.Count; i++)
        {
            State state = _states[i];
            UpdateStateDecay(state);
            UpdateStateCondition(state);

        }
    }

    private void UpdateStateDecay(State state)
    {
        state.ProcessDecay();
    }

    private void UpdateStateCondition(State state)
    {
        StateCondition newCondition = state.CalculateCondition();
        //Debug.Log(newCondition);
        if (newCondition != state.Condition)
        {
            state.Condition = newCondition;
            Task task = new Task(this, state);
            state.OnStateConditionChange.Raise(task);
        }
    }

}
