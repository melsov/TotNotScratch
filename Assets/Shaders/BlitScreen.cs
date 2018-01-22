using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class BlitScreen : MonoBehaviour
{
    public bool shouldBlit;
    [SerializeField]
    protected Material normalScreenMaterial;
    [SerializeField]
    protected Material effectMaterial;
}

