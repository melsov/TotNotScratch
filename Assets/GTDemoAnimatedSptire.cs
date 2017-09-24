using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class GTDemoAnimatedSptire : GameThing
{
    protected override void keyDown(KeyCode kc) {
        if(kc == KeyCode.W) {
            gtAnimator.setIntegerParam("ActionState", 2);
            jump();
        }
        else if(kc == KeyCode.E) {
            gtAnimator.die();
            //gtAnimator.hadoken();
        }
    }

    private void jump() {
        
    }
}
