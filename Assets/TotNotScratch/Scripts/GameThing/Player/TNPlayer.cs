using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DamageInfo
{
    public int damage;
    public Transform source;
}

public struct GetPointsInfo
{
    public int points;
    public Transform source;
}

public class TNPlayer : GameThing {


    [SerializeField]
    private int maxHealth = 1;
    public int health { get; private set; }

    public int points { get; private set; }

    private Vector3 spawnPoint;

    protected override void awake() {
        base.awake();
        spawnPoint = transform.position;
        resetPlayer();
    }

    private void resetPlayer() {
        health = maxHealth;
        transform.position = spawnPoint;
        transform.rotation = Quaternion.identity;

    }

    public void getPoints(GetPointsInfo getPointsInfo) {
        points += getPointsInfo.points;
        print(points);
    }

    public void takeDamage(DamageInfo damageInfo) {
        health -= damageInfo.damage;
        if(health <= 0) {
            die();
        }
    }

    private void die() {
        waitRealtimeThen(3f, () => {
            resetPlayer();
        });
    }


}
