using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class IntervalCallback : MonoBehaviour
{

    public static IntervalCallback Attach(Transform parent) {

        Transform callbackFolder = parent.Find("IntervalCallbacks");
        if(!callbackFolder) {
            GameObject callbackFolderGO = new GameObject("IntervalCallbacks");
            callbackFolderGO.transform.SetParent(parent);
            callbackFolder = callbackFolderGO.transform;
        }
        GameObject holder = new GameObject(string.Format("IntervalCallback_{0}", callbackFolder.childCount));
        holder.transform.SetParent(callbackFolder);
        return holder.AddComponent<IntervalCallback>();
    }

    [SerializeField]
    private float interval;
    private int repeats;
    private Action callback;
    private bool waitInRealTime;

    public void setup(float interval, Action callback, int repeats = -1, bool waitInRealTime = false) {
        this.interval = interval;
        this.repeats = repeats;
        this.callback = callback;
        this.waitInRealTime = waitInRealTime;
    }

    public void commence() {
        StartCoroutine(_commence());
    }

    private IEnumerator wait() {
        if (waitInRealTime) {
            yield return new WaitForSecondsRealtime(interval);
        } else {
            yield return new WaitForSeconds(interval);
        }
    }

    private IEnumerator _commence() {
        for (int i = 0; ; ++i) {
            yield return wait();
            if(callback != null) {
                callback.Invoke();
            }
            if (repeats > 0 && i >= repeats) {
                break;
            }
        }
    }
}
