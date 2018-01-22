using System;
using System.Collections.Generic;
using UnityEngine;

public interface HasWatchable<T>
{
    Watchable<T> getWatchable();
}

public class Watchable<T>
{
    protected T t;
    public virtual T _value {
        get {
            return t;
        }
        set {
            t = value;
            foreach(Action<T> onValueSet in onValueSets) {
                onValueSet.Invoke(t);
            }
        }
    }

    private List<Action<T>> onValueSets = new List<Action<T>>();
    public void addAction(Action<T> onValueSet) {
        if (!onValueSets.Contains(onValueSet)) {
            onValueSets.Add(onValueSet);
        }
    }

    public Watchable(T t) {
        this.t = t;
    }

    public static implicit operator T(Watchable<T> watchable) { return watchable._value; }

    public static implicit operator bool(Watchable<T> watchable) { return watchable != null; }
}


public class RecordableFloat
{
    private float f;

    public static implicit operator float(RecordableFloat rf) { return rf.f; }
    public static implicit operator RecordableFloat(float f) { return new RecordableFloat() { f = f }; }

    private RecordableFloat() { }

    public override bool Equals(object obj) {
        if(obj is RecordableFloat) {
            return ((RecordableFloat)obj).f == f;
        }
        return base.Equals(obj);
    }
    public override int GetHashCode() {
        return base.GetHashCode();
    }

}

public class WatchablePlayerPrefFloat<T> : Watchable<T> where T : RecordableFloat
{
    public string key { get; private set; }
    public WatchablePlayerPrefFloat(string key, T t) : base(t) {
        this.key = key;
    }

    public override T _value {
        get {
            return base._value;
        }

        set {
            if(!t.Equals(value)) {
                PlayerPrefs.SetFloat(key, value);
            }
            base._value = value;
        }
    }
}