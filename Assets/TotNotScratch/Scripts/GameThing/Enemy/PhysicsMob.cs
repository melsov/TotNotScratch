using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VctorExtensions;

public class PhysicsMob : PhysicsObject
{

	protected Collider2D colldr;
	protected ContactFilter2D[] conFilters = new ContactFilter2D[8];
	protected Vector2 dir;
	[SerializeField]
	protected float maxSpeed = 2f;
	private Action _computeVelocity;
	protected ContactFilter2D terrainContactFilter;
	protected Vector2 downAndRight;
	protected ObstactleInfo obstacleInfo;

	[SerializeField]
	protected bool avoidFallingOffLedges = true;

	[SerializeField]
	protected bool debugDrawGizmos = false;

    protected List<RaycastHit2D> mobCastList = new List<RaycastHit2D>();
    ContactFilter2D mobConFilter = new ContactFilter2D();

	protected override void OnEnable()
	{
		base.OnEnable();
		downAndRight = new Vector2(Mathf.Sin(Mathf.PI / 4f), -Mathf.Sin(Mathf.PI / 4f));
		colldr = GetComponent<Collider2D>();
		obstacleInfo = new ObstactleInfo()
		{
			hitList = new List<RaycastHit2D>(),
			aggregateVelocity = Vector2.zero
		};
	}

	protected override void Start()
	{
		base.Start();
		terrainContactFilter.useTriggers = false;
        terrainContactFilter.SetLayerMask(LayerMask.GetMask("GameThingTerrain"));
		terrainContactFilter.useLayerMask = true;

        mobConFilter.useTriggers = false;
        mobConFilter.SetLayerMask(LayerMask.GetMask("GameThingPhysics"));

		dir = Vector2.right * (UnityEngine.Random.Range(0f, 1f) > .5 ? 1f : -1f);
		StartCoroutine(waitUntilGrounded());
	}

	private IEnumerator waitUntilGrounded()
	{
		_computeVelocity = () => { };
		while(!grounded) { yield return new WaitForFixedUpdate(); }
		_computeVelocity = mobVelocity;
	}

	protected override void ComputeVelocity()
	{
		_computeVelocity();
	}

	protected void mobVelocity()
	{
		float aheadDistance = (colldr.bounds.extents.x + .4f) * Mathf.Sign(dir.x);
        //Check forwards
        Vector2 look = new Vector2(aheadDistance, 0f);
		obstacleInfo.mostOpposingNormal = dir;
		obstacleInfo.mostOpposingDot = dir.dot(obstacleInfo.mostOpposingNormal);
		GetRayObstacles(colldr.bounds.center, look);

		if(obstacleInfo.mostOpposingDot < 0f) 
		{
			dir *= -1f;
		}
		else
		{
			bool shouldTurn = false;
			if (avoidFallingOffLedges)
			{
				Vector2 start = colldr.bounds.center.xy() + Vector2.right * aheadDistance;
				look = Vector2.down * (colldr.bounds.extents.y + .4f);
				GetRayObstacles(start, look);
				shouldTurn = obstacleInfo.nothingFound;
			}
			if(shouldTurn)
			{
				dir *= -1f;
			} else
			{
                PhysicsMob other = willHitAMob();
                if (other) {
                    dir.x = Mathf.Sign(transform.position.x - other.transform.position.x);
                }
				//TODO: use old GetObstacles if needed
			}
			
		}
		targetVelocity = dir * maxSpeed;
	}

    PhysicsMob willHitAMob() {
        CastForMobs(alongGround * deltaPosition.x);
        PhysicsMob result = null;
        foreach(RaycastHit2D rh in mobCastList) { 
            result = rh.collider.GetComponentInParent<PhysicsMob>();
            if(result) {
                break;
            }
        }
        return result;
    }

    void CastForMobs(Vector2 move) {
        float distance = move.magnitude;

        if (distance > minMoveDistance) {
            int count = rb.Cast(move, mobConFilter, hitBuffer, distance + shellRadius);
            mobCastList.Clear();
            for (int i = 0; i < count; i++) {
                mobCastList.Add(hitBuffer[i]);
            }
        }
    }
   

    protected struct ObstactleInfo
	{
		public List<RaycastHit2D> hitList;
		public Vector2 aggregateVelocity;
		public Vector2 mostOpposingNormal;
		public float mostOpposingDot;

		public bool nothingFound { get { return hitList.Count == 0; } }
	}

	void GetRayObstacles(Vector2 start, Vector2 lookDirection)
	{
		int count = Physics2D.Raycast(start, lookDirection, terrainContactFilter, hitBuffer, lookDirection.magnitude);
		obstacleInfo.hitList.Clear();
		for (int i = 0; i < count; ++i)
		{
			obstacleInfo.hitList.Add(hitBuffer[i]);
		}

		foreach (RaycastHit2D hit in obstacleInfo.hitList)
		{
			if(hit.collider.CompareTag("Player")) { continue; }

			float dot = dir.dot(hit.normal);
			if(dot < obstacleInfo.mostOpposingDot)
			{
				obstacleInfo.mostOpposingDot = dot;
				obstacleInfo.mostOpposingNormal = hit.normal;
			}

			gizmoRayHits.Add(hit);
		}
	}

	void GetTerrainObstacles(Vector2 lookDirection)
	{
		float distance = lookDirection.magnitude;

		if (distance > minMoveDistance)
		{
			int count = rb.Cast(lookDirection, terrainContactFilter, hitBuffer, distance + shellRadius);
			obstacleInfo.aggregateVelocity = velocity;
			obstacleInfo.hitList.Clear();
			for (int i = 0; i < count; i++)
			{
				hitBufferList.Add(hitBuffer[i]);
			}

			for (int i = 0; i < hitBufferList.Count; i++)
			{
				Vector2 currentNormal = hitBufferList[i].normal;
				//DBUG
				gizmoRayHits.Add(hitBufferList[i]);

				float projection = Vector2.Dot(obstacleInfo.aggregateVelocity, currentNormal);
				if (projection < 0)
				{
					obstacleInfo.aggregateVelocity -= projection * currentNormal;
				}
			}

		}
	}

	private List<RaycastHit2D> gizmoRayHits = new List<RaycastHit2D>();

	private void OnDrawGizmos()
	{
		if(!debugDrawGizmos) { return; }

		//Gizmos.color = Color.green;
		//drawCenteredGizmoInDirection(obstacleInfo.aggregateVelocity);
		//Gizmos.color = Color.red;
		//drawCenteredGizmoInDirection(groundNormal);	

		int i = 0;
		Gizmos.color = C32Util.darksMod(i++);
		drawCenteredGizmoInDirection(dir);
		foreach(RaycastHit2D hit in gizmoRayHits)
		{
			Gizmos.color = C32Util.darksMod(i++);	
			drawCenteredGizmoInDirection(hit.normal * 3f);
			drawCubeMarker(hit.point);
			Gizmos.color = C32Util.darksMod(i++);
			drawCubeMarker(hit.centroid);
		}

		gizmoRayHits.Clear();
	}

	private void drawCenteredGizmoInDirection(Vector3 _dir)
	{
		Gizmos.DrawLine(transform.position, transform.position + _dir);
	}
	
	private void drawCubeMarker(Vector2 global, float size = .3f)
	{
		Gizmos.DrawWireCube(global, new Vector3(size, size, size));
	}

}
