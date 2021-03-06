﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using VctorExtensions;

public class BreakableBlock : MonoBehaviour
{
    [SerializeField]
    private string breakSound = "wooden-ball";
    [SerializeField]
    private ParticleSystem particles;

    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerPlatformerController pl = collision.GetComponent<PlayerPlatformerController>();

        if (pl) {
            Vector2 dif = transform.position - pl.transform.position;
            if (dif.y > 0f) {
                if (Mathf.Abs(dif.x) < GetComponent<Collider2D>().bounds.extents.x * 1.2f) {
                    particles.Play();
                    ComponentHelper.FindAnywhereOrAdd<AudioManager>().play(breakSound);
                    Destroy(gameObject);
                }

            }
        }
    }
}
