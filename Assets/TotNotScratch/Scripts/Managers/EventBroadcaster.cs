using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class EventBroadcaster 
{
    public static void broadcast(string eventName, object parameter = null) {
        broadcast<Transform>(eventName, parameter);
    }

    public static void broadcast<T>(string eventName, object parameter = null) where T : Component {
        foreach(T t in UnityEngine.Object.FindObjectsOfType<T>()) {
            t.BroadcastMessage(eventName, parameter, SendMessageOptions.DontRequireReceiver);
        }
    }
}
