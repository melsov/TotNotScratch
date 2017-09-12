using UnityEngine;

[System.Serializable]
public struct MLRange
{
    public float start, end;

    public MLRange(float start, float end) {
        this.start = end < start ? end : start;
        this.end = end < start ? start : end;
    }

    public float length { get { return end - start; } }

    public bool contains(float x) {
        return x > start && x < end;
    }

    public bool contains(MLRange other) {
        return start < other.start && end > other.end;
    }

    public static MLRange MLRange01() { return new MLRange(0f, 1f); }
}
