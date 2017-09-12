using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;
using System.Collections;

public class GTButton : GameThing
{
    [SerializeField]
    private Color normalColor;
    [SerializeField]
    private Color clickColor;

    [SerializeField]
    private UnityEvent onClick;

    public int note;

    protected override void start() {
        normalColor = srendrr.color;
        clickColor = ColorHelper.Brighten(normalColor, .5f);
    }

    protected override void mouseDown(VectorXY global) {
        base.mouseDown(global);
        AudioManager.Instance.note(note);
        srendrr.color = clickColor;
    }

    protected override void mouseUp(VectorXY global) {
        base.mouseUp(global);
        srendrr.color = normalColor;
        onClick.Invoke();
    }

    public void highlight(float seconds) {
        StartCoroutine(_highlight(seconds));
    }
    
    public void unhighlight() {
        srendrr.color = normalColor;
    }

    private IEnumerator _highlight(float seconds) {
        srendrr.color = clickColor;
        AudioManager.Instance.note(note);
        yield return new WaitForSecondsRealtime(seconds);
        srendrr.color = normalColor;
    }


}
