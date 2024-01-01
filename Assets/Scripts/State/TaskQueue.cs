using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Object/Task Queue")]
public class TaskQueue : ScriptableObject
{
    public StateTaskEvent OnNewTaskQueued;

    [SerializeField]
    private List<Task> _data;

    public void Enqueue(Task item)
    {
        if (_data.Contains(item))
        {
            throw new System.InvalidOperationException("This task is already in queue");
        }
        _data.Add(item);
        _data.Sort((Task task1, Task task2) => task1.Priority - task2.Priority);
        OnNewTaskQueued?.Raise(item);
    }

    public Task Dequeue()
    {
        if (_data.Count == 0)
        {
            throw new System.InvalidOperationException("The queue is empty");
        }
        Task item = _data[0];
        _data.RemoveAt(0);
        return item;
    }

    public void Remove(Task task)
    {
        _data.Remove(task);
    }

    public int Count => _data.Count;

    public bool IsEmpty => _data.Count == 0;
}
