using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using System.Collections;

public class Announcer : PhoenixLikeSingleton<Announcer>
{
    private Text _text;
    private Text text {
        get {
            if(!_text) {
                _text = panel.GetComponentInChildren<Text>();
            }
            return _text;
        }
    }

    [SerializeField, Header("Drag a Panel with a Text component")]
    [Header("attached to itself or a child")]
    private RectTransform _panel;
    private RectTransform panel {
        get {
            if(!_panel) {
                _panel = Instantiate<RectTransform>(Resources.Load<RectTransform>("CanvasUI/Panel"));
                _panel.SetParent(canvas.transform);
            }
            return _panel;
        }
    }
    
    private Canvas _canvas;
    private Canvas canvas {
        get {
            _canvas = FindObjectOfType<Canvas>();
            if (!_canvas) {
                _canvas = Instantiate<Canvas>(Resources.Load<Canvas>("CanvasUI/Canvas"));
            }
            return _canvas;
        }
    }

    private bool alreadyAnnouncing;

    private void Start() {
        //panel = text.transform.parent.GetComponent<RectTransform>();

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
