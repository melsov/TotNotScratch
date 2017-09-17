using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ViewPortHelper : PhoenixLikeSingleton<ViewPortHelper>
{

    public Vector2 viewPortGlobalSize {
        get {
            return  Camera.main.ScreenToWorldPoint(Camera.main.pixelRect.size) - Camera.main.ScreenToWorldPoint(Vector3.zero) ;
        }
    }

    public Rect viewPortGlobal {
        get {
            return new Rect(Camera.main.ScreenToWorldPoint(Vector3.zero), viewPortGlobalSize);
        }
    }

    public bool containsGlobal(Vector3 global) {
        return viewPortGlobal.Contains(global);
    }
}
