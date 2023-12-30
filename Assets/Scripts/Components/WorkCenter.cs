using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(StatefulObject))]
public class WorkCenter : MonoBehaviour
{
    StatefulObject _statefulObject;
    [SerializeField]
    List<WorkSlot> _workSlots = new List<WorkSlot>();
    //[SerializeField]
    //WorkSlotSet _workSlotSet;

    //private void Awake()
    //{
    //    foreach(WorkSlot slot in _workSlots)
    //    {
    //        _workSlotSet.Items.Add(slot);
    //    }
    //}

    private void Start()
    {
        _statefulObject = GetComponent<StatefulObject>();
    }

    public WorkSlot GetAvailableSlot()
    {
        foreach (WorkSlot slot in _workSlots)
        {
            if (slot.isAvailable) return slot;
        }
        return null;
    }

}
