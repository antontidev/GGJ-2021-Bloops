using UnityEngine;
using UnityEngine.Events;
using UnityEditor.SceneManagement;

/// <summary>
/// Base class for collision actions
/// </summary>
/// <typeparam name="T">Enter event type</typeparam>
/// <typeparam name="T1">Exit event type</typeparam>
public abstract class Action<T, T1> : ScriptableObject where T  
                                    : UnityEvent       where T1 
                                    : UnityEvent {
    public T OnEnterEvent;
    public T1 OnExitEvent;

    public Tag tag;

    public abstract void OnInnerEnterAction(GameObject gamaObj, Vector3 meetPos);

    public abstract void OnInnerExitAction(Vector3 byePos);
}
