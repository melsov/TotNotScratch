using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private AudioManager audioManager;
    private BlitScreen deathScreen;
    [SerializeField]
    private float deathScreenSeconds = 3f;

    private void Awake() {
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
    }

    private void Start() {
        deathScreen = Camera.main.GetComponent<BlitScreen>();
        deathScreen.shouldBlit = false;
    }

    #region levels-scenes

    public void deathSequence() {
        audioManager.play("derderder");
        deathScreen.shouldBlit = true;
        StartCoroutine(waitThenReload(deathScreenSeconds));
    }

    private IEnumerator waitThenReload(float deathScreenSeconds) {
        yield return new WaitForSecondsRealtime(deathScreenSeconds);
        reloadActiveScene();
    }

    private Scene activeScene {
        get {
            return SceneManager.GetActiveScene();
        }
    }

    private bool isFinalScene {
        get {
            return SceneManager.sceneCountInBuildSettings - 1 == activeScene.buildIndex;
        }
    }

    private void loadNextScene() {
        if (!isFinalScene) {
            SceneManager.LoadScene(activeScene.buildIndex + 1);
        }
    }

    private void loadFirstScene() {
        SceneManager.LoadScene(0);
    }

    private void reloadActiveScene() {
        SceneManager.LoadScene(activeScene.buildIndex);
    }

    #endregion
}
