using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

public class GroundedDetector : MonoBehaviour
{
    private Collider2D colldr;
    private Collider2D[] overlappingColliders;
    private ContactFilter2D contactFilter;

    private void Awake() {
        overlappingColliders = new Collider2D[7];
        contactFilter = new ContactFilter2D();
        contactFilter.layerMask = LayerMask.GetMask("GameThingPhysics", "GameThingTerrain");
        colldr = GetComponent<Collider2D>();
        Assert.IsTrue(colldr && colldr.isTrigger, "GroundedDector needs a trigger collider");
    }

    public bool isGrounded() {
        
        foreach(Collider2D coll in overlappingColliders) {
            if(isGroundedWith(coll)) {
                return true;
            }
        }
        return false;
    }

    private float verticalHedge {
        get {
            return colldr.bounds.size.y;
        }
    }

    private bool isGroundedWith(Collider2D coll) {
        if(!coll) { return false; }
        //Vector3 closest = coll.bounds.ClosestPoint(colldr.bounds.center);
        Vector3 upperCorner = coll.bounds.center + coll.bounds.extents;

        return (colldr.bounds.center.y > upperCorner.y - colldr.bounds.size.y);
    }

    private void checkOverlaps() {
        for(int i=0; i< overlappingColliders.Length; ++i) {
            overlappingColliders[i] = null;
        }
        colldr.OverlapCollider(contactFilter, overlappingColliders);
    }

    private void OnTriggerEnter2D(Collider2D collider) {
        checkOverlaps();
    }

    private void OnTriggerExit2D(Collider2D collision) {
        checkOverlaps();
    }

}
