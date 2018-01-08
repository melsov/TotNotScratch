using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ViewPortHelper : MonoBehaviour
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

    public VectorXY normalizedPosition(Vector2 v) {
        Rect global = viewPortGlobal;
        VectorXY vv = v - global.min;
        vv = vv / new VectorXY(global.size);
        return vv;
    }

    public VectorXY normalizedCenteredness(VectorXY v) {
        v = normalizedPosition(v.toVector2);
        v = ((new VectorXY(.5f, .5f) - v).abs() * 2f).clamped01();
        return v;
    }
}
