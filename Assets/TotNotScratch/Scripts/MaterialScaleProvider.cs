using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MaterialScaleProvider : MonoBehaviour
{

    

    public void setMaterialScale() {
        Material m = GetComponent<Renderer>().material;
        m.SetVector("_Scale", new Vector4(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z, 0));
    }

    private void Start() {
        setMaterialScale();
    }
}
