using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class QuestionMarkBlock : MonoBehaviour
{

	[SerializeField]
	Transform spawnPosition;

	[SerializeField]
	Transform target;

    Animator animtor;
	Watchable<bool> activated = new Watchable<bool>(false);

    private void OnEnable()
	{
		animtor = GetComponent<Animator>();
	}

    private void OnTriggerEnter2D(Collider2D collision) 
	{
		if (activated._value) { return; }

        if (collision.GetComponent<PlayerPlatformerController>())
		{
            if (collision.transform.position.y < transform.position.y) {
                spawn();
            }
		}
	}


	private void spawn()
	{
		activated._value = true;
		Transform tar = Instantiate<Transform>(target);
		tar.position = spawnPosition.position;
        animtor.SetBool("Inactive", true);
	}
}
