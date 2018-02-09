using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevelGoal : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision) {
        PlayerPlatformerController pl = collision.GetComponent<PlayerPlatformerController>();
        print("t enter");
        if(pl) {
            pl.winLevel();
        }
    }
}
