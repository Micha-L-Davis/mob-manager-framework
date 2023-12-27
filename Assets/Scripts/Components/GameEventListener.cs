using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{
    public GameEvent Event;
    public UnityEvent Response = new UnityEvent();

    private void OnEnable()
    { Event?.RegisterListener(this); }

    private void OnDisable()
    { Event?.UnregisterListener(this); }

    public void OnEventRaised()
    { Response.Invoke(); }
}

public abstract class GameEventListener<T> : MonoBehaviour
{
    public GameEvent<T> Event;
    public UnityEvent<T> Response;

    private void OnEnable()
    { Event?.RegisterListener(this); }

    private void OnDisable()
    { Event?.UnregisterListener(this); }

    public void OnEventRaised(T t)
    { Response.Invoke(t); }
}