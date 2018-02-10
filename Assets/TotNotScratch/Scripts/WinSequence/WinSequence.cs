using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using VctorExtensions;

public class WinSequence : MonoBehaviour
{

    [SerializeField]
    Transform winBackground;
    [SerializeField]
    Text winText;
    [SerializeField]
    string winSoundName = "tock.wav";
    [SerializeField]
    Transform itineraryParent;
    [SerializeField]
    Transform endSequenceTarget;
    [SerializeField]
    Collider2D winImageFrame;

    [SerializeField]
    Transform ejectToTarget;
    [SerializeField]
    float ejectSpeed = 3f;
    [SerializeField]
    float exploreWinBackgroundSpeed = 3f;


    PlayerPlatformerController player;
    [SerializeField]
    private float zoomOutSeconds = 5f;

    private void OnEnable() {
        player = GetComponent<PlayerPlatformerController>();
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
        PlCameraFollow camFollow = FindObjectOfType<PlCameraFollow>();
        camFollow.centerPlayerMode = true;

        winBackground.gameObject.SetActive(true);
        winBackground.GetComponent<BeInvisibleOnEnable>().beInvisible(false);
        itineraryParent.gameObject.SetActive(true);
        itineraryParent.GetComponent<BeInvisibleOnEnable>().beInvisible(false);

        //To eject target
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        Vector2 startPos = rb.position;
        float frames = (ejectToTarget.position.xy() - startPos).magnitude / ejectSpeed; // ejectSeconds / Time.fixedDeltaTime;
        foreach(int i in Enumerable.Range(0, (int)frames)) {
            Vector2 pos = Vector2.Lerp(startPos, ejectToTarget.position, i / frames);
            rb.MovePosition(pos);
            yield return new WaitForFixedUpdate();
        }

        //explore win background
        List<Transform> itinerary = new List<Transform>();
        foreach (Transform child in itineraryParent) {
            itinerary.Add(child);
        }

        itinerary.Add(endSequenceTarget);

        foreach (Transform destination in itinerary) {
            Vector2 dif = destination.position.xy() - rb.position;
            frames = dif.magnitude / exploreWinBackgroundSpeed; // exploreWinBackgroundSpeed / Time.fixedDeltaTime;
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
