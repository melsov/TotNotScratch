using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public class GroundedPhysicsDetector : MonoBehaviour
{
  

    [SerializeField]
    private Collider2D colldr;

    private ContactPoint2D[] contactPoints;
    private ContactFilter2D filter;

    [Space(8)]
    [SerializeField, Header("How steep can ground be? (Inverted value)"), Range(-1f, .9999f)]
    [Header("1 only flat ground. 0 walls count. -1 ceilings count.")]
    [Space(3)]
    private float steepTolerance = .1f;

    [SerializeField]
    private DebugLight dbugLight;

    private void Awake() {
        contactPoints = new ContactPoint2D[6];
        filter.layerMask = LayerMask.GetMask("GameThingPhysics", "GameThingTerrain");
    }

    public IEnumerable<ContactPoint2D> getOverlappingColliders() {
        int count = colldr.GetContacts(filter, contactPoints);
        for(int i=0; i< count; ++i) {
            yield return contactPoints[i];
        }
    }

    public GroundedInfo isGrounded() {

        foreach (var cp in getOverlappingColliders()) {
            if (Vector2.Dot(Vector2.up, cp.normal) > steepTolerance) {
                return new GroundedInfo(true, cp.normal);
            }
        }
        return new GroundedInfo(false, Vector2.zero);
    }

}
