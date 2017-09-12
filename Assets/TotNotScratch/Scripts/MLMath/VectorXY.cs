using UnityEngine;
using System.Collections;
using System;

public struct VectorXY  {

    private Vector2 v;

    public static VectorXY maxValue = new VectorXY(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue));
    public static Vector3 maxVector3 = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
    public static VectorXY fakeNull = new VectorXY(float.MaxValue - 14, float.MaxValue - 6); // maxValue;
    internal static VectorXY right = new VectorXY(1f, 0f);

    public bool isFakeNull() {
        return this == fakeNull;
    }

    public float x {
        get { return v.x; }
        set { v.x = value; }
    }

    public float y {
        get { return v.y; }
        set { v.y = value; }
    }

    public Vector3 vector3() {
        return vector3(0);
    }

    public Vector3 vector3(float z) {
        return new Vector3(v.x, v.y, z);
    }

    public Vector2 toVector2 {
        get {
            return v;
        }
    }

    public VectorXY swapped {
        get {
            return new VectorXY(v.y, v.x);
        }
    }


    public float magnitudeSquared {
        get { return v.x * v.x + v.y * v.y; }
    }

    public float magnitude {
        get { return v.magnitude; }
    }

    public VectorXY invertedMagnitudeSafe {
        get { return  this / Mathf.Max(.01f, dot(this)); }
    }

    public static VectorXY max(VectorXY a, VectorXY b) {
        return new VectorXY(a.x > b.x ? a.x : b.x, a.y > b.y ? a.y : b.y);
    }

    public static VectorXY min(VectorXY a, VectorXY b) {
        return new VectorXY(a.x < b.x ? a.x : b.x, a.y < b.y ? a.y : b.y);
    }

    public VectorXY sign() { return new VectorXY(Mathf.Sign(x), Mathf.Sign(y)); }

    public float slope { get { return y / x; } }

    public VectorXY normalized {
        get { return new VectorXY(v.normalized); }
    }

    public VectorXY normal {
        get {
            VectorXY ized = normalized;
            return new VectorXY(-ized.y, ized.x);
        }
    }

    public VectorXY(Vector3 v3) {
        v = new Vector2(v3.x, v3.y);
    }

    public VectorXY(Vector2 v2) {
        v = v2;
    }

    public VectorXY(float n) {
        v = new Vector2(n, n);
    }

    public VectorXY(float _x, float _y) {
        v = new Vector2(_x, _y);
    }

    public float dot(VectorXY other) {
        return Vector2.Dot(v, other.v);
    }

    public bool sympatheticDirection(VectorXY other) {
        return dot(other) > 0f;
    }

    public float angleDegrees() {
        return angleRadians() * Mathf.Rad2Deg;
    }

    public float angleRadians() {
        //swapping components so that the resulting angle "a" satisfies:
        // (cos(a), sin(a)) == this vector normalized
        VectorXY p_vector2 = swapped; 
        if (p_vector2.x < 0) {
            return Mathf.PI * 2f - Mathf.Atan2(p_vector2.x, p_vector2.y) * -1f;
        } else {
            return Mathf.Atan2(p_vector2.x, p_vector2.y);
        }
    }

    public static VectorXY operator +(VectorXY a, VectorXY b) {
        return new VectorXY(a.v + b.v);
    }
    public static VectorXY operator -(VectorXY a, VectorXY b) {
        return new VectorXY(a.v - b.v);
    }
    public static bool operator == (VectorXY a, VectorXY b) {
        return a.v == b.v;
    }
    public static bool operator != (VectorXY a, VectorXY b) {
        return a.v != b.v;
    }
    public static VectorXY operator *(VectorXY a, float b) {
        return new VectorXY(a.v * b);
    }
    public static VectorXY operator *(float b, VectorXY a) {
        return new VectorXY(a.v * b);
    }
    public static VectorXY operator *(VectorXY a, VectorXY b) {
        return new VectorXY(a.x * b.x, a.y * b.y);
    }
    public static VectorXY operator /(VectorXY v, float f) {
        return new VectorXY(v.x / f, v.y / f);
    }

    public static bool operator >(VectorXY a, VectorXY b) {
        return a.x > b.x && a.y > b.y;
    }
    public static bool operator <(VectorXY a, VectorXY b) {
        return a.x < b.x && a.y < b.y;
    }

    public static implicit operator VectorXY(Vector3 v) { return new VectorXY(v); }
    public static implicit operator bool (VectorXY v) { return !v.isFakeNull(); }

    public override bool Equals(object obj) {
        return v.Equals(obj);
    }
    public override int GetHashCode() {
        return v.GetHashCode();
    }

    public override string ToString() {
        return string.Format("VecXY {0} , {1}", x, y);
    }

    public static bool DebugContainsNaNCheck(params VectorXY[] vs) {
        int nans = 0;
        foreach(VectorXY v in vs) {
            if (v.containsNaN()) { nans++; }
        }
        if (nans > 0) { Debug.Log(string.Format("{0} were NaN", nans)); }
        return nans > 0;
    }

    public bool containsNaN() {
        return float.IsNaN(x) || float.IsNaN(y);
    }

}
