using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WinSequence : MonoBehaviour
{

    [SerializeField]
    Transform winBackground;
    [SerializeField]
    Text winText;
    [SerializeField]
    string winSoundName = "tock.wav";
    [SerializeField]
    Transform endSequenceTarget;
    [SerializeField]
    Collider2D winImageFrame;

    [SerializeField]
    Transform ejectToTarget;
    [SerializeField]
    float ejectSeconds = 3f;
    [SerializeField]
    float exploreWinBackgroundSeconds = 8f;


    PlayerPlatformerController player;
    [SerializeField]
    private float zoomOutSeconds = 5f;

    private void OnEnable() {
        player = GetComponent<PlayerPlatformerController>();
        winBackground.gameObject.SetActive(false);
        winText.enabled = false;
    }

    public void win() {
        StartCoroutine(playSequence(() =>
        {
            FindObjectOfType<GameManager>().winLevel();
        }));
    }

    public IEnumerator playSequence(Action onCompleted) {
        player.releaseControl();
        winBackground.gameObject.SetActive(true);
        PlCameraFollow camFollow = FindObjectOfType<PlCameraFollow>();
        camFollow.centerPlayerMode = true;


        //To eject target
        float frames = ejectSeconds / Time.fixedDeltaTime;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 startPos = rb.position;
        foreach(int i in Enumerable.Range(0, (int)frames)) {
            Vector2 pos = Vector2.Lerp(startPos, ejectToTarget.position, i / frames);
            rb.MovePosition(pos);
            yield return new WaitForFixedUpdate();
        }

        //explore win background
        List<Transform> itinerary = new List<Transform>();
        itinerary.AddRange(endSequenceTarget.GetComponentsInChildren<Transform>());
        itinerary.Add(endSequenceTarget);
        foreach (Transform destination in itinerary) {
            frames = exploreWinBackgroundSeconds / Time.fixedDeltaTime;
            startPos = rb.position;
            foreach (int i in Enumerable.Range(0, (int)frames)) {
                rb.MovePosition(Vector2.Lerp(startPos, new Vector2(destination.position.x, destination.position.y), i / frames));
                yield return new WaitForFixedUpdate();
            }
        }

        winText.enabled = true;
        frames = zoomOutSeconds / Time.fixedDeltaTime;
        float startScale = FindObjectOfType<ViewPortHelper>().viewPortGlobal.size.x;

        foreach (int i in Enumerable.Range(0, (int)frames)) {
            camFollow.zoomToContainWidth(Mathf.Lerp(startScale, winImageFrame.bounds.size.x, i / frames));
            yield return new WaitForFixedUpdate();
        }
        FindObjectOfType<AudioManager>().play(winSoundName);

        onCompleted();
    }


}
