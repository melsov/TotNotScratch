﻿Shader "TotNotScratch/RoundedBox"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Radius ("_Radius", Range(0,1.0)) = 0.5 
		_Scale ("_Scale", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { 	"Queue"="Transparent"				
			"RenderType"="Transparent" 
		}
		LOD 100

		//ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			// #pragma multi_compile_fog

			#define ROOT2 1.41421356237
			#define ROOT_PT5 0.70710678118
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				// float3 worldPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed _Radius;
			fixed4 _Scale;

			
			
			v2f vert (appdata v)
			{
				v2f o;
				// o.worldPos = mul (unity_ObjectToWorld, v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			fixed4 roundCorners(fixed4 col, float2 uv) {
				uv = uv * _Scale.xy;
				fixed2 d = min(uv, _Scale.xy - uv);
			 	fixed2 t = saturate(d > _Radius * ROOT_PT5);
				fixed c =  dot(t,t) + (length( _Radius*ROOT_PT5 - d) <  _Radius*ROOT_PT5);
				return fixed4(col.rgb, c);
			}

			fixed2 scaleUV(fixed2 uv) {
				return fmod(uv * _Scale.xy, 1);
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 uv = scaleUV(i.uv);
				fixed4 col = tex2D(_MainTex, uv );
				col = roundCorners(col, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
