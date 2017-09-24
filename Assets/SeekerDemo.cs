using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SeekerDemo : GameThing
{
    [SerializeField]
    private Transform target;

    [SerializeField]
    private float rotationSlerp = .5f;

    protected override void lateStart() {
        setBackground("Stripes");
    }

    protected override void fixedUpdate() {
        slerpLookAt(target.position, rotationSlerp);
        moveForward();
    }
}
