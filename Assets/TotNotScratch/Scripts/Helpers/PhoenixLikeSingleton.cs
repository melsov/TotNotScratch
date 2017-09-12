using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoenixLikeSingleton<T> : Singleton<T> where T : MonoBehaviour {

    private void Awake() {
        applicationIsQuitting = false;
    }
}
