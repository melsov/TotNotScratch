using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DebugLight : MonoBehaviour
{

    public SpriteRenderer srendrr;
    public Color onColor = Color.green;
    public Color offColor = Color.gray;

    private void Awake() {
        srendrr = GetComponent<SpriteRenderer>();
    }

    public void setColor(Color c) {
        srendrr.color = c;
    }

    public void setOn(bool isOn) {
        setColor(isOn ? onColor : offColor);
    }
}
