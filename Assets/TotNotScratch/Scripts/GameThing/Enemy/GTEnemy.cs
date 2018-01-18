using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField]
    ParticleSystem squishParticles;
    [SerializeField]
    string squishSoundName = "small-laser";
    AudioManager audioManager;


    bool alive = true;

    private void Awake() {
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
        rb = GetComponent<Rigidbody2D>();
        colldr = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        srendrr = GetComponent<SpriteRenderer>();
        setupProbes();
    }

    private void Start() {
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
        if (anim) {
            anim.SetBool(walkingAnimatorParameter, Mathf.Abs(move.x) > .01f);
        }
    }

    protected void handleHeadProbeEvent(ProbeEventInfo probeEventInfo) {
        if(probeEventInfo.isEnterEvent) {
            if( probeEventInfo.colldr.CompareTag("Player")) {
                TNPlayer player = probeEventInfo.colldr.GetComponent<TNPlayer>();
                player.getPoints(new GetPointsInfo() { points = 1 });

                squish();
            }
        }
    }

    private void squish() {
        alive = false;
        audioManager.play(squishSoundName);
        squishParticles.Play();
        StartCoroutine(dieAnimation());
    }

    private IEnumerator dieAnimation() {
        foreach(int i in Enumerable.Range(0, 12)) {
            Vector3 scl = transform.localScale;
            scl.y *= .5f;
            transform.localScale = scl;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
    }
}
