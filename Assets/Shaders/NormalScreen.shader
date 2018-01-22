Shader "BlitScreen/NormalScreen" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		
	}
	SubShader {
		Pass {
			Cull Off 
			ZWrite Off 
			ZTest Always

			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#include "UnityCG.cginc"

			#define time _Time[1]



			uniform sampler2D _MainTex;
 
			fixed4 frag (v2f_img i) : COLOR {
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}