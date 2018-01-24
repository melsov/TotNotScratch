using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class PhysicsEnemy : PhysicsMob
{

	[SerializeField]
	Probe head;
	[SerializeField]
	Probe body;
	private bool alive = true;
	AudioManager audioManager;
	[SerializeField]
	ParticleSystem squishParticles;
	[SerializeField]
	string squishSoundName = "tock";
	[SerializeField]
	private float damage = 1;

	protected ContactPoint2D[] contactPoints = new ContactPoint2D[12];
	protected List<ContactPoint2D> contactPointList = new List<ContactPoint2D>();

	protected override void Start()
	{
		base.Start();
		audioManager = ComponentHelper.FindAnywhereOrAdd<AudioManager>();
		head.addTriggerEventListener(handleHeadProbeEvent);
		body.addTriggerEventListener(handleBodyProbeEvent);
	}

	protected bool getContactPointWith(Collider2D coll, out ContactPoint2D result)
	{
		int count = colldr.GetContacts(contactPoints);
		contactPointList.Clear();
		for(int i=0; i<count; ++i)
		{
			contactPointList.Add(contactPoints[i]);
		}
		foreach(ContactPoint2D cp in contactPointList)
		{
			if (cp.collider == coll)
			{
				result = cp;
				return true;
			}
		}
		result = default(ContactPoint2D);
		return false;
	}

	protected virtual void handleBodyProbeEvent(ProbeEventInfo info)
	{
		if(!alive) { return; }

		if (info.isEnterEvent)
		{
			PlayerPlatformerController player = info.colldr.GetComponent<PlayerPlatformerController>();

			if(player)
			{
				bool shouldDealDamage = !head.colldr.IsTouching(info.colldr); // isOverlappingCollider(info.colldr);
				if (shouldDealDamage)
				{
					ContactPoint2D cp;
					bool foundPoint = getContactPointWith(info.colldr, out cp);
					if(foundPoint)
					{
						shouldDealDamage = cp.normal.y < 0f && Mathf.Abs( cp.normal.y) > Mathf.Abs( cp.normal.x);
					}
				}
				if (shouldDealDamage)
				{
					player.takeDamage(new DamageInfo() { damage = damage });
				} else
				{
					squish(player);
				}
			} 
		}
	}

	protected virtual void handleHeadProbeEvent(ProbeEventInfo info)
	{

		if(info.isEnterEvent)
		{
			PlayerPlatformerController player = info.colldr.GetComponent<PlayerPlatformerController>();
			if(player)
			{
				squish(player);
			}
		}
	}


	private void squish(PlayerPlatformerController player)
	{
		player.getPoints(new GetPointsInfo() { points = 1 });
		alive = false;
		audioManager.play(squishSoundName);
		squishParticles.Play();
		StartCoroutine(dieAnimation());
	}

	private IEnumerator dieAnimation()
	{
		foreach (int i in Enumerable.Range(0, 12))
		{
			Vector3 scl = transform.localScale;
			scl.y *= .5f;
			transform.localScale = scl;
			yield return new WaitForFixedUpdate();
		}
		Destroy(gameObject);
	}
}
