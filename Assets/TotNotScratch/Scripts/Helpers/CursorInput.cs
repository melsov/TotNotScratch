using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;

public class CursorInput : PhoenixLikeSingleton<CursorInput> {
    protected CursorInput() { }

    public bool isDisabled;

    private RaycastHit rayHit;
    private CursorInputClient ci;

    private LayerMask layerMask;
    private int dragOverrideMask;

    void Awake () {
        layerMask = LayerMask.GetMask("NoCollisions", "GameThingPhysics"); // ~(LayerMask.GetMask("DragOverride") | LayerMask.GetMask("CogComponent"));
        dragOverrideMask = LayerMask.GetMask("DragOverride");
    }
	
	void Update () {
       
        if (isDisabled) { return; }

        /*
         * TODO: implement mouse hover awareness for highlightables (e.g. items)
         */
        cursorInteract();
	}

    private void cursorInteract() {
        if (Input.GetButtonDown("Fire1")) {
            // Don't interact with scene if pointer is over a canvas gameobject 
            if (!EventSystem.current.IsPointerOverGameObject()) { 
                ci = getInteractable<CursorInputClient>();
                if(ci) {
                    ci.mouseDown(mouseWorldPosition); 
                }
            }
        }
        if (Input.GetButton("Fire1")) {
            if (ci != null) {
                ci.drag(mouseWorldPosition); 
            }
        }
        if (Input.GetButtonUp("Fire1")) {
            if (ci != null) {
                ci.mouseUp(mouseWorldPosition);
            }
            releaseItems();
        }

    }

    public Vector3 mouseWorldPosition {
        get {
            Vector3 v = Input.mousePosition;
            return Camera.main.ScreenToWorldPoint(new Vector3(v.x, v.y, Camera.main.nearClipPlane + 4));
        }
    }

    public void releaseItems() {
        ci = null;
    }

    private T getInteractable<T>() where T : MonoBehaviour {
        releaseItems();
        T result = null;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out rayHit, 100f, dragOverrideMask)) { //try to get a drag override interactable first
            result = rayHit.collider.GetComponentInParent<T>();
        } 
        else  //2D
        {
            RaycastHit2D rayHit2D = Physics2D.Raycast(ray.origin, ray.direction, 100f, dragOverrideMask);
            if(rayHit2D) {
                result = rayHit2D.collider.GetComponentInParent<T>();
            }
        }

        if (result == null) {
            result = getHighestZInteractable<T>(ray);// getHighestZInteractable(ray);
        }
        if (result == null) {
            RaycastHit2D rayHit2D = Physics2D.Raycast(ray.origin, ray.direction, 100f, layerMask);
            if (rayHit2D) {
                result = rayHit2D.collider.GetComponentInParent<T>();
            }
            if(!result) { return null; }
        }
        return result;
    }

    private T getHighestZInteractable<T>(Ray ray) where T : MonoBehaviour {
        HashSet<Collider> ciColliders = CollidersPiercedBy<T>(ray, 5, layerMask, true);
        T result = null;
        foreach(Collider col in ciColliders) {
            T ci = col.GetComponentInParent<T>();
            if (ci) {
                if (result == null || result.transform.position.z < ci.transform.position.z) {
                    result = ci;
                }
            }
        }
        return result;
    }

    public static HashSet<Collider> CollidersPiercedBy<T>(Ray ray, int depth, LayerMask layerMask, bool wantCursorInteraction) where T : MonoBehaviour {
        RaycastHit rayHit;
        HashSet<Collider> ciColliders = new HashSet<Collider>();
        for(int i = 0; i < 5; ++i) { 
            if (Physics.Raycast(ray, out rayHit, 100f, layerMask)) {

                //TODO: iron-out awkward logic here. Could use a dynamic type? instead of the 'want' bool?
                if (wantCursorInteraction && rayHit.collider.GetComponentInParent<T>() == null) {
                    break;
                } else {
                    ray = RaycastMaster.RaySlightlyBelowHit(ray, rayHit);
                    ciColliders.Add(rayHit.collider);
                }
            } else {
                break;
            }
        }
        return ciColliders;
    }

    public bool IsColliderUnderCursor(Collider coll) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask collMask = LayerMask.GetMask(LayerMask.LayerToName(coll.gameObject.layer));
        HashSet<Collider> colliders = CollidersPiercedBy<MonoBehaviour>(ray, 5, collMask, false);
        return colliders.Contains(coll);
    }

    public static bool CursorRayhit(Collider coll, out RaycastHit rh) {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        LayerMask collMask = LayerMask.GetMask(LayerMask.LayerToName(coll.gameObject.layer));
        int i = 0;
        do {
            if (Physics.Raycast(ray, out rh, 100f, collMask)) {
                if (rh.collider == coll) {
                    return true;
                } else {
                    ray = RaycastMaster.RaySlightlyBelowHit(ray, rh);
                }
            } else {
                break;
            }
        } while (++i < 5);
        return false;
    }


}

public static class RaycastMaster
{
    private static RaycastHit hit;

    public static T ComponentUnderCursor<T>(Collider ignore) where T : MonoBehaviour { return ComponentUnderCursor<T>(ignore, LayerMask.GetMask("Default")); }

    public static T ComponentUnderCursor<T>(LayerMask limitToLayerMask) where T : MonoBehaviour { return ComponentUnderCursor<T>(null, limitToLayerMask); }

    public static T ComponentUnderCursor<T>(Collider ignore, LayerMask limitToLayerMask) where T : MonoBehaviour {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        int i = 0;
        T result = null;
        do {
            if(Physics.Raycast(ray, out hit, 100f, limitToLayerMask)) {
                if(hit.collider != ignore) {
                    result = hit.collider.GetComponentInParent<T>();
                    if(result) { return result; }
                }
                ray = RaySlightlyBelowHit(ray, hit);
            }
        } while (++i < 5);
        return null;
    }
    public static Ray RaySlightlyBelowHit(Ray ray, RaycastHit rayCastHit) {
        return new Ray(rayCastHit.point + ray.direction * .01f, ray.direction);
    }
}