using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "New Event", menuName = "Event", order = 1)]
public class GameEvent : ScriptableObject
{
    private UnityEvent unityEvent;

    [ContextMenu("Trigger event")]
    public void Invoke(){
        unityEvent.Invoke();
    }

    public void Subscribe(UnityAction callback){
        unityEvent.AddListener(callback);
    }

    public void Unsubscribe(UnityAction listener){
        unityEvent.RemoveListener(listener);
    }
}