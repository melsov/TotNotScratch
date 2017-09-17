using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class FixedLocalPosition : MonoBehaviour
{
    [HideInInspector]
    public Vector3 local;

    private void Start() {
        local = transform.localPosition;
    }

    private void Update() {
        transform.position = transform.parent.position + local;
    }
}
