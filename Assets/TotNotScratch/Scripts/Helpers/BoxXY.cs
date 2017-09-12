using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxXY : MonoBehaviour {

    private Collider _coll;
    private Collider coll {
        get {
            if(!_coll) {
                _coll = GetComponentInChildren<Collider>();
            }
            return _coll;
        }
    }

    public VectorXY min {
        get {
            return new VectorXY(coll.bounds.min);
        }
    }

    public VectorXY max {
        get {
            return new VectorXY(coll.bounds.max);
        }
    }

    public bool contains(Vector3 v) {
        return contains(new VectorXY(v));
    }

    public bool contains(VectorXY v) {
        return v > min && v < max;
    }

    public bool overlaps(BoxXY other) {
        return contains(other.min) || contains(other.max);
    }
}
