using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerController : PhysicsObject {

    public float maxSpeed = 7;
    public float jumpTakeOffSpeed = 7;
    [SerializeField, Range(0f, 1f)]
    private float xSpeedInAirDamper = .5f;

    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private AudioManager audioManager;

    protected Vector2 lastGroundedMoveX;
    [SerializeField]
    private string jumpSound = "blip";
    [SerializeField]
    private string deathSound = "derderder";

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

    // Use this for initialization
    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
        stats = ComponentHelper.FindAnywhereOrAdd<Platformer.PlayerStats>();
    }

	private void Start()
	{
		groundPoint = rb.position;
		stats.health.addAction((int i) =>
		{
			print(i);
			animator.SetInteger("Health", i);
		});
        stats.health._value = startHealth;

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
		groundPointContactFilter.SetLayerMask(LayerMask.NameToLayer("GameThingTerrain"));
		groundPointContactFilter.useLayerMask = true;
	}

	private void setGroundPoint()
	{
		hitList.Clear();
		int count = Physics2D.Raycast(rb.position, Vector2.down, groundPointContactFilter, gpHitBuffer, 30f);
		for (int i =0; i<count; ++i)
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

        if (stats.health._value <= 0) {
            FindObjectOfType<GameManager>().deathSequence();
        }
    }

    protected override void ComputeVelocity() {
        Vector2 move = Vector2.zero;

        if(grounded) {
            move.x = Input.GetAxis("Horizontal");
            lastGroundedMoveX = move;
        } else {
            move.x = Mathf.Lerp(lastGroundedMoveX.x, Mathf.Clamp(lastGroundedMoveX.x + Input.GetAxis("Horizontal") * xSpeedInAirDamper, -1f, 1f), .35f);
        }

        if (Input.GetButtonDown("Jump") && grounded) {
            velocity.y = jumpTakeOffSpeed;
            audioManager.play(jumpSound);
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

    public void getPoints(GetPointsInfo info) {
        stats.points._value += info.points;
    }

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(groundPoint, Vector3.one * .5f);
	}
}
