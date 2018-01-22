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

    protected ContactPoint2D[] contactPoints = new ContactPoint2D[12];


    bool alive = true;
    private RaycastHit2D[] hitBuffer = new RaycastHit2D[12];

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
        colldr = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        srendrr = GetComponent<SpriteRenderer>();
        setupProbes();
    }

    private void Start() {
        StartCoroutine(fallToGround());
    }

    protected bool isGrounded() {
        return false;
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


    protected virtual void setupProbes() {
        probeSet.left.addTriggerEventListener(handleLeftRightProbeEvent);
        probeSet.right.addTriggerEventListener(handleLeftRightProbeEvent);
        probeSet.feetLeft.addTriggerEventListener(handleFootProbeEvent);
        probeSet.feetRight.addTriggerEventListener(handleFootProbeEvent);
        probeSet.head.addTriggerEventListener(handleHeadProbeEvent);
        probeSet.body.addTriggerEventListener(handleBodyProbeEvent);
    }

    protected virtual void handleFootProbeEvent(ProbeEventInfo probeEventInfo) {
        if(!probeEventInfo.isEnterEvent && !probeEventInfo.probe.isOverlappingTerrain()) {
            setHorizontalDirection(move.x * -1f);
        }
    }

    protected virtual void handleLeftRightProbeEvent(ProbeEventInfo probeEventInfo) {
        if (probeEventInfo.isEnterEvent && probeEventInfo.probe.isOverlappingTerrain()) {
            setHorizontalDirection(move.x * -1f);
        }
    }

    protected virtual void handleBodyProbeEvent(ProbeEventInfo probeEventInfo) {
        if (alive && probeEventInfo.isEnterEvent && probeEventInfo.colldr.CompareTag("Player")) {
            PlayerPlatformerController player = probeEventInfo.colldr.GetComponent<PlayerPlatformerController>();
            if (player) {
                player.takeDamage(new DamageInfo() { damage = 1f });
            }
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

    protected virtual void handleHeadProbeEvent(ProbeEventInfo probeEventInfo) {
        if (probeEventInfo.isEnterEvent && probeEventInfo.colldr.CompareTag("Player")) {
            Vector3 towardsFeet = probeEventInfo.colldr.bounds.extents * .8f; //extents = half bounds size
            Vector3 dif = probeEventInfo.colldr.bounds.center - towardsFeet - probeSet.head.colldr.bounds.center;
            if(dif.y > 0) {
                probeEventInfo.colldr.GetComponent<PlayerPlatformerController>().getPoints(new GetPointsInfo() { points = 1 });
                squish();
            }
        }
    }

    //handle non trigger collisions with our main body collider (not probes)
    private void OnCollisionEnter2D(Collision2D collision) {
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
