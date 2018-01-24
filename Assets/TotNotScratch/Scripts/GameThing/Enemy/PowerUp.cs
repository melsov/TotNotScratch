using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class PowerUp : PhysicsEnemy
{

	protected override void handleBodyProbeEvent(ProbeEventInfo probeEventInfo)
	{
		give(probeEventInfo);
	}

	private void give(ProbeEventInfo info)
	{
		if (info.colldr.CompareTag("Player"))
		{
			PlayerPlatformerController player = info.colldr.GetComponent<PlayerPlatformerController>();
			player.takeDamage(new DamageInfo() { damage = -1f });
			Destroy(gameObject);
		}
		
	}

	protected override void handleHeadProbeEvent(ProbeEventInfo probeEventInfo)
	{
		give(probeEventInfo);
	}
}
