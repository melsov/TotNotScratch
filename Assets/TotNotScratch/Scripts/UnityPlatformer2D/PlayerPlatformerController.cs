using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

	[System.Serializable]
	public class Sounds
	{
		public string jump = "blip";
		public string mushroom = "spacey-power-up";
		public string damage = "laser-shot";
	}
	public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    [SerializeField, Range(0f, 1f)]
    private float xSpeedInAirDamper = .5f;
	[SerializeField]
	private bool deadDropIfNoXInput = true;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioManager audioManager;

    protected Vector2 lastGroundedMoveX;
	[SerializeField]
	private Sounds sounds;

    private Platformer.PlayerStats stats;

    [SerializeField]
    private int maxHealth = 2;
	[SerializeField]
	private int startHealth = 1;

	private RaycastHit2D[] gpHitBuffer = new RaycastHit2D[6];
	private List<RaycastHit2D> hitList = new List<RaycastHit2D>();
	private ContactFilter2D groundPointContactFilter;

	public Vector2 groundPoint { get; private set; }
	public Rigidbody2D rigidBodyy { get { return rb; } }

	private bool canControl = true;

	// Use this for initialization
	protected override void OnEnable()
	{
		base.OnEnable();
    }

	private void setup()
	{
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
        stats = ComponentHelper.FindAnywhereOrAdd<Platformer.PlayerStats>();
	}

	protected override void Start()
	{
		base.Start();
		setup();
		stats.health.addAction((int i) =>
		{
			animator.SetInteger("Health", i);			
		});
        stats.health._value = startHealth;

		groundPoint = rb.position;
		setupRayHit();
		StartCoroutine(updateGroundPoint());
	}

	private IEnumerator updateGroundPoint()
	{

		while(true)
		{
			setGroundPoint();
			yield return new WaitForSeconds(.1f);
		}
	}

	private void setupRayHit()
	{
		groundPointContactFilter.useTriggers = false;
		groundPointContactFilter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); // LayerMask.NameToLayer("GameThingTerrain")));
		groundPointContactFilter.useLayerMask = true;
	}

	private void setGroundPoint()
	{
		hitList.Clear();
		int count = Physics2D.Raycast(rb.position, Vector2.down, groundPointContactFilter, gpHitBuffer, 30f);
		for (int i = 0; i < count; ++i)
		{
			hitList.Add(gpHitBuffer[i]);
		}		
		foreach(RaycastHit2D hit in hitList)
		{
			if (hit.collider.CompareTag("Player")) { continue; }
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GameThingTerrain"))
			{
				groundPoint = hit.point;
				return;
			}
		}
		groundPoint = rb.position;
	}


	public void takeDamage(DamageInfo damageInfo) {
        stats.health._value -= (int) damageInfo.damage;
		if (damageInfo.damage > 0)
		{
			audioManager.play(sounds.damage);
		}
		else
		{
			audioManager.play(sounds.mushroom);
		}

		if (stats.health._value <= 0) {
			canControl = false;
            FindObjectOfType<GameManager>().deathSequence();
        }
    }

    protected override void ComputeVelocity() {

		if(!canControl) { return; }

        Vector2 move = Vector2.zero;

        if(grounded) {
            move.x = Input.GetAxis("Horizontal");
            lastGroundedMoveX = move;
        } else {
			if (!deadDropIfNoXInput || Mathf.Abs(Input.GetAxis("Horizontal")) > 0f)
			{
				move.x = Mathf.Lerp(lastGroundedMoveX.x, Mathf.Clamp(lastGroundedMoveX.x + Input.GetAxis("Horizontal") * xSpeedInAirDamper, -1f, 1f), .35f);
			} else
			{
				move.x = .012f * Mathf.Sign(lastGroundedMoveX.x);
			}
		}

        if (Input.GetButtonDown("Jump") && grounded) {
            StartCoroutine(debugCrudeVelocity());
            velocity.y = jumpTakeOffSpeed;
            audioManager.play(sounds.jump);
        } else if (!Input.GetButton("Jump")) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.95f;
            }
        }

        bool flipSprite = (spriteRenderer.flipX ? (move.x > 0.01f) : (move.x < 0.01f));
        if (flipSprite) {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    private IEnumerator debugCrudeVelocity() {
        for(int i = 0; i < previousPositions.size; ++i) {
            print(previousPositions[previousPositions.size - 1]);
            yield return new WaitForFixedUpdate();
        }
        Vector2 cvel = getCrudeVelocity();
        print("crude: " + cvel.ToString());
        print(getRecentVelocity());
    }

    protected override void nudgeRigidbody(Vector2 nudge)
	{
		if (canControl)
		{
			base.nudgeRigidbody(nudge);
		}
	}

	public void getPoints(GetPointsInfo info) {
        stats.points._value += info.points;
    }

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(groundPoint, Vector3.one * .5f);
	}
}
