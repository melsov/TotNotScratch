using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FixedRotation : MonoBehaviour
{
    private Quaternion ro;
    [SerializeField]
    private Vector3 euler;

    public void setRotation(Vector3 euler) {
        this.euler = euler;
        _setRotation();
    }

    private void Awake() {
        _setRotation();
    }

    private void _setRotation() {
        ro = Quaternion.Euler(euler);
    }

    private void Update() {
        transform.rotation = ro;
    }
}
