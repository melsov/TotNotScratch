using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VctorExtensions;

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

    [SerializeField]
    private bool flipSpriteHorizontally;

	private RaycastHit2D[] gpHitBuffer = new RaycastHit2D[6];
	private List<RaycastHit2D> hitList = new List<RaycastHit2D>();
	private ContactFilter2D groundPointContactFilter;

	public Vector2 groundPoint { get; private set; }
	public Rigidbody2D rigidBodyy { get { return rb; } }

	private bool canControl = true;
    private bool shouldSquishBounce;

    [SerializeField]
    private float squishBounceSpeed = 12f;

    private RayCastHitFind downGroundFoundInfo;
    private RayCastHitFind upGroundFoundInfo;

    private List<RaycastHit2D> lodgeInTerrainPoints = new List<RaycastHit2D>();
    RayCastHitFind lodgedInfo;
    Abyss abyss;
    private bool startedDyingAlready;

    // Use this for initialization
    protected override void OnEnable()
	{
		base.OnEnable();
    }

	private void setup()
	{
        startedDyingAlready = false;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
        stats = ComponentHelper.FindAnywhereOrAdd<Platformer.PlayerStats>();
        abyss = FindObjectOfType<Abyss>();
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
			yield return new WaitForSeconds(.2f);
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
        findGround(Vector2.down, out downGroundFoundInfo);
        bool gotGround = downGroundFoundInfo.found;

        if(downGroundFoundInfo.found) {
            groundPoint = downGroundFoundInfo.hit.point;

            //piggyback on groundPoint check: we may have already found a lodged in ground situation?
            downGroundFoundInfo.lodged = _colldr.bounds.Contains2D(downGroundFoundInfo.hit.point);
        } else {
            //check in abyss
            if (abyss.hasEntered(rb.position)) {
                die();
            }
        }
        escapeLodgedInTerrain();

        if (!gotGround) {
            groundPoint = rb.position;
        }
	}

    struct RayCastHitFind
    {
        public RaycastHit2D hit;
        public bool found;
        public bool lodged;
    }

    //TODO: func that provides a description of which part of player is in terrain
    private void escapeLodgedInTerrain() {
        if (downGroundFoundInfo.found && downGroundFoundInfo.lodged) {
            Vector2 move = Vector2.Scale(downGroundFoundInfo.hit.point - _colldr.bounds.min.xy(), Vector2.up);
            rb.position += move;
            return;
        }

        findGround(Vector2.up, out upGroundFoundInfo, 5f);
        if(upGroundFoundInfo.found) {
            upGroundFoundInfo.lodged = _colldr.bounds.Contains2D(upGroundFoundInfo.hit.point);
            if (upGroundFoundInfo.lodged) {
                Vector2 move = Vector2.Scale(upGroundFoundInfo.hit.point - _colldr.bounds.max.xy(), Vector2.up);
                rb.position += move;
            }
        }
    }

    private void findGround(Vector2 lookDirection, out RayCastHitFind found, float dist = 30f) {
		hitList.Clear();
        found = default(RayCastHitFind);
		int count = Physics2D.Raycast(rb.position, lookDirection, groundPointContactFilter, gpHitBuffer, dist);
		for (int i = 0; i < count; ++i)
		{
			hitList.Add(gpHitBuffer[i]);
		}		
		foreach(RaycastHit2D hit in hitList)
		{
			if (hit.collider.CompareTag("Player")) { continue; }
			if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GameThingTerrain"))
			{
                found.hit = hit;
                found.found = true;
                return;

			}
		}
        return;
    }

    private void getOutOfTerrain(RaycastHit2D found) {
        

    }
    //private void checkLodgedInTerrain() {
    //    hitList.Clear();
    //    lodgeInTerrainPoints.Clear();
    //    int count = rb.Cast(Vector2.up, contactFilter, gpHitBuffer, 0f);
    //    for(int i = 0; i < count; ++i) {
    //        hitList.Add(gpHitBuffer[i]);
    //    }
    //    foreach(RaycastHit2D hit in hitList) {
    //        if(hit.collider.CompareTag("Player")) { continue; }
    //        if(hit.collider.gameObject.layer == LayerMask.NameToLayer("GameThingTerrain")) {
    //            lodgeInTerrainPoints.Add(hit);
    //        }
    //    }
    //}


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
            die();
        }
    }

    private void die() {
        if(startedDyingAlready) { return; }
        startedDyingAlready = true;
        canControl = false;
        FindObjectOfType<GameManager>().deathSequence();

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

        if(shouldSquishBounce) {
            shouldSquishBounce = false;
            velocity.y = squishBounceSpeed;
        }
        else if (Input.GetButtonDown("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
            audioManager.play(sounds.jump);
        } else if (!Input.GetButton("Jump")) {
            if (velocity.y > 0) {
                velocity.y = velocity.y * 0.95f;
            }
        }

        if(Mathf.Abs(Input.GetAxis("Horizontal")) > 0f) {
            spriteRenderer.flipX = (Input.GetAxis("Horizontal") < 0f) != flipSpriteHorizontally;
        }
        //bool flipSprite = (spriteRenderer.flipX != flipSpriteHorizontally ? (move.x > 0.01f) : (move.x < 0.01f));
        //if (flipSprite) {
        //    spriteRenderer.flipX = !spriteRenderer.flipX;
        //}

        animator.SetBool("grounded", grounded);
        animator.SetFloat("velocityX", Mathf.Abs(velocity.x) / maxSpeed);

        targetVelocity = move * maxSpeed;
    }

    protected override void nudgeRigidbody(Vector2 nudge)
	{
		if (canControl)
		{
			base.nudgeRigidbody(nudge);
		}
	}

    public void handleSquish(GetPointsInfo info) {
        shouldSquishBounce = true;
        stats.points._value += info.points;
    }

	public void getPoints(GetPointsInfo info) {
        stats.points._value += info.points;
    }

	private void OnDrawGizmos()
	{
		//Gizmos.color = Color.yellow;
		//Gizmos.DrawWireCube(groundPoint, Vector3.one * .5f);
	}
}
