using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

public class CallbackHelper : PhoenixLikeSingleton<CallbackHelper>
{
    public void waitThenInvoke(float seconds, Action callback) {
        StartCoroutine(_waitThenInvoke(seconds, callback));
    }

    private IEnumerator _waitThenInvoke(float seconds, Action callback) {
        yield return new WaitForSecondsRealtime(seconds);
        if(callback != null) {
            callback.Invoke();
        }
    }
}
