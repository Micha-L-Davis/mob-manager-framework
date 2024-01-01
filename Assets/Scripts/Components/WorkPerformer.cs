using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Events;
using UnityEngine.Events;

[RequireComponent(typeof(StateResponder))]
public class WorkPerformer : MonoBehaviour
{
    [SerializeField]
    List<TaskQueue> _workQueues = new List<TaskQueue>();

    [SerializeField]
    Task _activeTask;
    float _baseLabor = 0.5f;

    StateResponder _responderComponent;
    private void Awake()
    {
        // create Unity Event components for each responsibilty
        foreach (TaskQueue queue in _workQueues)
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
        for (int i = 0; i < _workQueues.Count; i++)
        {
            StateTaskEvent gameEvent = _workQueues[i].OnNewTaskQueued;
            StateChangeEventListener listener = listeners[i];
            if (listener == null || gameEvent == null)
            {
                Debug.LogError("Cannot initialize event listener with null data");
            }
            listener.Event = gameEvent;

            System.Type[] types = new[] { typeof(Task) };
            System.Reflection.MethodInfo targetInfo = UnityEvent.GetValidMethodInfo(this, nameof(EvaluateTask), types);
            UnityAction<Task> methodDelegate = System.Delegate.CreateDelegate(typeof(UnityAction<Task>), this, targetInfo, true) as UnityAction<Task>;
            UnityEventTools.AddPersistentListener(listener.Response, methodDelegate);
            listener.enabled = false;
            listener.enabled = true;
        }
    }

    private void EvaluateTask(Task task)
    {
        if (_activeTask != null)
        {
            // If the incoming task is from a task queue lower than the current task's queue, ignore
            if (_workQueues.IndexOf(task.TargetObject.WorkQueue) > _workQueues.IndexOf(_activeTask.TargetObject.WorkQueue)) return;
            // If the incoming task is lower than or equal in priority to the current task, ignore
            if (task.Priority < _activeTask.Priority) return;
            // If incoming task refers to the same state variable as the current task, ignore
            if (task.State.Variable == _activeTask.State.Variable) return;
        }

        task.TargetObject.WorkQueue.Remove(task);
        _activeTask = task;

        Work(task);
    }

    private void Work(Task task)
    {
        // find a work position on the target object
        WorkCenter workCenter = task.TargetObject.GetComponent<WorkCenter>();
        if (task.TargetObject == null) return;
        WorkSlot slot = workCenter.GetAvailableSlot();
        // if distance is too great, signal the responder component
        if (Vector3.Distance(this.transform.position, task.TargetObject.transform.position) > 0.25f)
        {
            _responderComponent.Respond(task.TargetObject.gameObject);
        }
        // if distance is near, apply labor to the work position
        else
        {
            slot.ApplyLabor(task.State.Variable, _baseLabor);
        }
    }

    void CompleteTast(Task task)
    {
        if (task.State.Condition >= task.CompletionCondition)
        {
            _activeTask = null;
        }
    }

    private void FindWork()
    {
        for (int i = 0; i < _workQueues.Count; i++)
        {
            TaskQueue queue = _workQueues[i];

            if (queue.IsEmpty) continue;

            _activeTask = queue.Dequeue();
        }
    }

    private void Update()
    {
        if (_activeTask == null) FindWork();
    }
}
