using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhoenixLikeSingleton<T> : Singleton<T> where T : MonoBehaviour {

    private void Awake() {
        applicationIsQuitting = false;
        SceneManager.sceneLoaded += onSceneLoaded;
    }

    protected virtual void onSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {

    }
}
