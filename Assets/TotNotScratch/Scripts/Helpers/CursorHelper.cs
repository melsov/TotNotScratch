using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class CursorHelper 
{
    public static Vector3 cursorGlobalXYCameraNearClipPlaneZ {
        get {
            return Camera.main.ScreenToWorldPoint(
                            new Vector3(Input.mousePosition.x, 
                            Input.mousePosition.y,
                            Camera.main.nearClipPlane));

        }
    }
    
    public static VectorXY cursorGlobalXY {
        get {
            return new VectorXY(cursorGlobalXYCameraNearClipPlaneZ);
        }
    }


    public static float angleDegreesWithCursorXY(Vector3 pos) {
        return (new VectorXY(Input.mousePosition) - new VectorXY(Camera.main.WorldToScreenPoint(pos))).angleDegrees(); // dif.angleDegrees();
    }

}
