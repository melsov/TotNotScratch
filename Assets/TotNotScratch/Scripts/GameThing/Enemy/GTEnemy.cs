using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTEnemy : MonoBehaviour {

    [SerializeField]
    ProbeSet probeSet;

    Animator anim;
    SpriteRenderer srendrr;

    [SerializeField]
    GroundedPhysicsDetector groundedDetector;

    [SerializeField, Header("match with walking/idle animator param")]
    string walkingAnimatorParameter = "Walking";

    Vector2 move = Vector2.right;
    [SerializeField]
    bool flipSprite;
    [SerializeField]
    float speed = 6f;
    Rigidbody2D rb;
    Collider2D colldr;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        colldr = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        srendrr = GetComponent<SpriteRenderer>();
        setupProbes();
        StartCoroutine(fallToGround());
    }

    private IEnumerator fallToGround() {
        rb.isKinematic = false;
        while(!groundedDetector.isGrounded()) {
            yield return new WaitForFixedUpdate();
        }
        rb.isKinematic = true;
        StartCoroutine(perambulate());
    }

    private IEnumerator perambulate() {
        while (true) {
            rb.MovePosition(rb.position + (move * speed * Time.deltaTime));
            yield return new WaitForFixedUpdate();
        }
    }


    private void setupProbes() {
        probeSet.left.addTriggerEventListener(handleLeftRightProbeEvent);
        probeSet.right.addTriggerEventListener(handleLeftRightProbeEvent);
        probeSet.feetLeft.addTriggerEventListener(handleFootProbeEvent);
        probeSet.feetRight.addTriggerEventListener(handleFootProbeEvent);
        probeSet.head.addTriggerEventListener(handleHeadProbeEvent);
    }

    protected void handleFootProbeEvent(ProbeEventInfo probeEventInfo) {
        if(!probeEventInfo.isEnterEvent && !probeEventInfo.probe.isOverlappingTerrain()) {
            setHorizontalDirection(move.x * -1f);
        }
    }

    protected void handleLeftRightProbeEvent(ProbeEventInfo probeEventInfo) {
        if (probeEventInfo.isEnterEvent && probeEventInfo.probe.isOverlappingTerrain()) {
            setHorizontalDirection(move.x * -1f);
        }
    }

    private void setHorizontalDirection(float v) {
        move.x = v;
        if (Mathf.Abs(move.x) > .01f) {
            srendrr.flipX = (v < 0) != flipSprite;
        }
        anim.SetBool(walkingAnimatorParameter, Mathf.Abs(move.x) > .01f);
    }

    protected void handleHeadProbeEvent(ProbeEventInfo probeEventInfo) {
        
    }


}
