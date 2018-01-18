using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;
using UnityEditor;

public struct GroundedInfo
{
    public bool grounded;
    public Vector2 groundNormal;

    public GroundedInfo(bool grounded, Vector2 groundNormal) {
        this.grounded = grounded;
        this.groundNormal = groundNormal;
    }

    public static implicit operator bool(GroundedInfo gi) { return gi.grounded; }
}

public class GroundedDetector : MonoBehaviour
{
    [SerializeField]
    private Collider2D triggerColldr;

    private Collider2D[] contactPoints;
    private ContactFilter2D filter;

    private Transform[] ignore;

    [Space(8)]
    [SerializeField, Header("How steep can ground be? (Inverted value)"), Range(-1f, .9999f)]
    [Header("1 only flat ground. 0 walls count. -1 ceilings count.")]
    [Space(3)]
    private float steepTolerance = .1f;

    [SerializeField]
    private DebugLight dbugLight;

    private void Awake() {
        contactPoints = new Collider2D[20];
        filter.layerMask = LayerMask.GetMask("GameThingPhysics", "GameThingTerrain");
        Assert.IsTrue(triggerColldr.isTrigger, "Collider2D needs to be a trigger");
        ignore = new Transform[0];
    }
    private void Start() {
        ignore = GetComponentsInChildren<Transform>();
    }

    public IEnumerable<Collider2D> getOverlappingColliders() {
        int count = triggerColldr.OverlapCollider(filter, contactPoints);
        bool skip;
        for(int i=0; i < count; ++i) {
            Collider2D coll = contactPoints[i];
            if(coll.transform == transform) { continue; }
            skip = false;
            foreach(Transform t in ignore) {
                if(coll.transform == t) { skip = true; break; }
            }
            if(skip) { continue; }

            yield return contactPoints[i];
        }
    }

    public GroundedInfo isGrounded() {

        foreach (Collider2D cp in getOverlappingColliders()) {
            ColliderDistance2D cd = triggerColldr.Distance(cp);
            //TODO: Laszlo is getting stuck in 2 square wide holes
            print(cd.normal.ToString());
            if (Vector2.Dot(Vector2.up * -1f, cd.normal) > steepTolerance) {
                dbugLight.setOn(true);
                return new GroundedInfo(true, cd.normal);
            }
        }
        dbugLight.setOn(false);
        return new GroundedInfo(false, Vector2.zero);
    }

    




}

