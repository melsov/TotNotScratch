using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsObject : MonoBehaviour {

    public float minGroundNormalY = .65f;
    public float gravityModifier = 1f;

    protected Vector2 targetVelocity;
    protected bool grounded;
    protected Vector2 groundNormal;
    protected Rigidbody2D rb;
    protected Vector2 velocity;
    protected ContactFilter2D contactFilter;
    protected RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
    protected List<RaycastHit2D> hitBufferList = new List<RaycastHit2D>(16);
    protected RingBuffer<Vector2> previousPositions = new RingBuffer<Vector2>(16);


    protected const float minMoveDistance = 0.001f;
    protected const float shellRadius = 0.01f;

    protected virtual void OnEnable() {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start() {
        contactFilter.useTriggers = false;
        contactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer));
        contactFilter.useLayerMask = true;
    }

    void Update() {
        targetVelocity = Vector2.zero;
        ComputeVelocity();
    }

    protected virtual void ComputeVelocity() {
    }

    public Vector2 alongGround { get { return new Vector2(groundNormal.y, -groundNormal.x); } }

    public Vector2 deltaPosition { get { return velocity * Time.deltaTime; } }

    public Vector2 getRecentVelocity() {
        Vector2 result = Vector2.zero;
        for(int i = 1; i < previousPositions.size; ++i) {
            result += (previousPositions[i] - previousPositions[i - 1]);
        }
        return result;
    }

    public Vector2 getCrudeVelocity() {
        return previousPositions[previousPositions.size - 2] - previousPositions[0];
    }

    void FixedUpdate() {
        velocity += gravityModifier * Physics2D.gravity * Time.deltaTime;
        velocity.x = targetVelocity.x;

        grounded = false;

        Vector2 _deltaPosition = deltaPosition;

        Vector2 move = alongGround * _deltaPosition.x;

        Movement(move, false);

        move = Vector2.up * _deltaPosition.y;

        Movement(move, true);
    }

    void Movement(Vector2 move, bool yMovement) {
        float distance = move.magnitude;

        if (distance > minMoveDistance) {
            int count = rb.Cast(move, contactFilter, hitBuffer, distance + shellRadius);
            hitBufferList.Clear();
            for (int i = 0; i < count; i++) {
                hitBufferList.Add(hitBuffer[i]);
            }

            for (int i = 0; i < hitBufferList.Count; i++) {
                Vector2 currentNormal = hitBufferList[i].normal;
                if (currentNormal.y > minGroundNormalY) {
                    grounded = true;
                    if (yMovement) {
                        groundNormal = currentNormal;
                        currentNormal.x = 0;
                    }
                }

                float projection = Vector2.Dot(velocity, currentNormal);
                if (projection < 0) {
                    velocity = velocity - projection * currentNormal;
                }

                float modifiedDistance = hitBufferList[i].distance - shellRadius;
                distance = modifiedDistance < distance ? modifiedDistance : distance;
            }


        }

		nudgeRigidbody(move.normalized * distance);
    }

	protected virtual void nudgeRigidbody(Vector2 nudge)
	{
        previousPositions.push(rb.position);
		rb.position += nudge;
	}

}
