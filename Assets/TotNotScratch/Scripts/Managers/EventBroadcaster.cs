using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EventBroadcaster : PhoenixLikeSingleton<EventBroadcaster>
{
    public void broadcast(string eventName, object parameter = null) {
        broadcast<Transform>(eventName, parameter);
    }

    public void broadcast<T>(string eventName, object parameter = null) where T : Component {
        foreach(T t in FindObjectsOfType<T>()) {
            t.BroadcastMessage(eventName, parameter, SendMessageOptions.DontRequireReceiver);
        }
    }
}
