Shader "BlitScreen/DeathScreen" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"
		
			uniform sampler2D _MainTex;
 
			fixed4 frag (v2f_img i) : COLOR {

				fixed4 base = tex2D(_MainTex, i.uv);		
				fixed4 skew = abs(.5 - base);
				fixed3 blackWhite = dot(skew, base) * .15 + .45; 
				return fixed4(blackWhite, 1);

			}
			ENDCG
		}
	}
}