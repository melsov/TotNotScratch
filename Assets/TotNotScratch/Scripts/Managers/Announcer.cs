using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System.Collections;

public class Announcer : PhoenixLikeSingleton<Announcer>
{
    [SerializeField]
    private Text text;

    private RectTransform panel;

    private bool alreadyAnnouncing;

    private void Start() {
        panel = text.transform.parent.GetComponent<RectTransform>();
        hide();
    }

    private void hide() {
        panel.gameObject.SetActive(false);
    }

    private void show() {
        panel.gameObject.SetActive(true);
    }

    public void announce(string words, float seconds = 2f, Action callback = null) {
        StartCoroutine(announceThenHide(words, seconds, callback));
    }

    private IEnumerator announceThenHide(string words, float seconds, Action callback) {
        while(alreadyAnnouncing) {
            yield return new WaitForSecondsRealtime(.1f);
        }
        alreadyAnnouncing = true;
        show();
        text.text = words;
        yield return new WaitForSecondsRealtime(seconds);
        if(callback != null) {
            callback.Invoke();
        }
        hide();
        alreadyAnnouncing = false;
    }
}
