using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

//attach to camera
public class CRTEffect : BlitScreen
{

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        if (shouldBlit) {
            Graphics.Blit(source, destination, effectMaterial);
        } else {
            Graphics.Blit(source, destination, normalScreenMaterial);
        }
    }


}
