using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GTRotater : GameThing
{
    protected override void keyDown(KeyCode kc) {
        if (kc == KeyCode.R) {
            changeColor(Color.red);
        }
    }

    protected override void start() {
        physicsWorksOnMe(true);
    }

    protected override void fixedUpdate() {
        lookAt(mousePosition);
        lerpTo(mousePosition);
    }

    protected override void collisionEnterWithSomethingTagged(TaggedCollision tag) {
        if(tag == "MildlyBad") {
            say("i hit a something mildly bad");
        }
    }
}
