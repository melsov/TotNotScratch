using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Assertions;

public class GroundedDetector : MonoBehaviour
{

    private Collider2D colldr;
    private Rigidbody2D rb;

    private ContactPoint2D[] contactPoints;
    private ContactFilter2D filter;

    [Space(8)]
    [SerializeField, Header("How steep can ground be? (Inverted value)"), Range(-1f, .9999f)]
    [Header("1 only flat ground. 0 walls count. -1 ceilings count.")]
    [Space(3)]
    private float steepTolerance = .1f;

    private void Awake() {
        contactPoints = new ContactPoint2D[20];
        colldr = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        filter.layerMask = LayerMask.GetMask("GameThingPhysics", "GameThingTerrain");
    }

    public IEnumerable<ContactPoint2D> getContactPoints() {
        int count = colldr.GetContacts(filter, contactPoints);
        for(int i=0; i< count; ++i) {
            yield return contactPoints[i];
        }
    }

    public bool isGrounded() {
        foreach (ContactPoint2D cp in getContactPoints()) {

            if (Vector2.Dot(Vector2.up, cp.normal) > steepTolerance) {
                return true;
            }

        }
        return false;
    }




}

