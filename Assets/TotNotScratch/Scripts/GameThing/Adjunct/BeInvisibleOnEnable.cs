using UnityEngine;
using System.Collections;

public class BeInvisibleOnEnable : MonoBehaviour
{

    private void OnEnable() {
        beInvisible(true);
    }

    public void beInvisible(bool invisible) {
        foreach(Renderer re in GetComponentsInChildren<Renderer>()) {
            re.enabled = !invisible;
        }
    }

}
