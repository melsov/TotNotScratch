using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Abyss : MonoBehaviour
{

    [SerializeField]
    private Transform surfaceYMarker;

    public float surfaceY { get { return surfaceYMarker.position.y; } }

    public bool hasEntered(Vector2 v) { return surfaceY > v.y; }

    private void OnDrawGizmos() {
        Gizmos.color = new Color(1f, 0f, 0f, .5f);
        float yy = 300;
        Gizmos.DrawCube(new Vector3(0f, surfaceY - yy), new Vector3(4000f, yy * 2f));
    }
}
