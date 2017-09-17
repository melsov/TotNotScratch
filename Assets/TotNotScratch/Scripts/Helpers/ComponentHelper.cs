using System;
using UnityEngine;

public static class ComponentHelper
{
    public static T AddIfNotPresent<T>(Transform trans) where T : Component {
        T thing = trans.GetComponent<T>();
        if (!thing) {
            thing = trans.gameObject.AddComponent<T>();
        }
        return thing;
    }
    public static T AddIfNotPresentInChildren<T>(Transform trans) where T : Component {
        T thing = trans.GetComponentInChildren<T>();
        if (!thing) {
            thing = trans.gameObject.AddComponent<T>();
        }
        return thing;
    }

    public static T FindInChildrenOrAddChildFromResourcesPrefab<T>(Transform trans, string resourcesRelPath, Vector3 childRelPos = default(Vector3)) where T : Component {
        T thing = trans.GetComponentInChildren<T>();
        if(!thing) {
            thing = UnityEngine.Object.Instantiate(Resources.Load<T>(resourcesRelPath));
            //thing.transform.position = trans.position + childRelPos;
            thing.transform.SetParent(trans);
            thing.transform.localPosition = childRelPos;
        }
        return thing;
    }

    public static T CreateGameObjectWithComponent<T>() where T : Component {
        GameObject go = new GameObject();
        return go.AddComponent<T>();
    }

    public static T GetComponentOnlyInChildren<T>(Transform trans) where T : Component {
        T result = null;
        foreach(Transform child in trans) {
            result = child.GetComponentInChildren<T>();
            if(result) { break; }
        }
        return result;
    }

    internal static void EnforceTriggerCollider(Transform transform) {
        Collider coll = transform.GetComponent<Collider>();
        if(coll is MeshCollider) {
            ((MeshCollider)coll).convex = true;
        }
        try {
            coll.isTrigger = true;
        } catch(Exception e) {
            MonoBehaviour.print(transform.name + " has a convex collider?? " + e.ToString());
        } 
    }
}