Shader "BlitScreen/DubScreen" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_interval ("_interval", Float) = 1.2
		_DropletXY("Droplet Center XY (z is time)", Vector) = (0, 0, 0, 0)	
		_ADropletXY("Second Droplet Center XY (z is time)", Vector) = (0, 0, 0, 0)
		_BDropletXY("Third Droplet Center XY (z is time)", Vector) = (0, 0, 0, 0)		
		_CDropletXY("Fourth Droplet Center XY (z i t)", Vector) = (0, 0, 0, 0)
		_UnityTime("_UnityTime", Float) = 0
		_AmpMult("_AmpMult", Float) = .04
		_DistAmpInfluence("_DistAmpInfluence", Float) = 3
		_FakeTime("_FakeTime Test", Float) = .5
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
			uniform sampler2D _MaskTex;
			
			half _interval;

			float4 _DropletXY;
			float4 _ADropletXY;
			float4 _BDropletXY;
			float4 _CDropletXY;
			half _UnityTime;
			fixed _AmpMult;
			half _DistAmpInfluence;
			float _FakeTime;

			// fixed4 Grid(fixed2 pos){
			// 	float moduloX= fmod(pos.x, .1);
			// 	float moduloY= fmod(pos.y, .1);
			// 	fixed2 mod = fmod(pos, .1)	;
			// 	fixed4 color;
			// 	// if(moduloX< .15 || moduloY< .15){
			// 		color.x=mod.x + .5;
			// 		color.y=mod.y + .5;
			// 		color.z=mod.y;
			// 		color.w=1;
			// 	// }
			// 	return color;
			// }

			fixed2 radialDisplace(fixed2 uv, fixed4 drop) {
				fixed2 n = normalize( drop.xy - uv);
				fixed dist = distance(drop.xy, uv);
				fixed inverseDist = 1 - dist;
				fixed fadeInvDist = saturate(.25 - dist);
				fixed timedif = (_UnityTime - drop.z);
				fixed pulse = saturate( dist + (_interval - timedif) / _interval );
				fixed enable = (pulse > 0);

				// fixed amp =  enable * pulse * pow(inverseDist, _DistAmpInfluence) * _AmpMult; 
				fixed amp =  fadeInvDist * enable * pulse  / pow(dist, _DistAmpInfluence) * _AmpMult; 

				return saturate( amp * sin(3.14159 * .5 * (fadeInvDist  + time) ) * n);
			}
 
			fixed4 frag (v2f_img i) : COLOR {

				fixed2 n = radialDisplace(i.uv, _DropletXY);
				// fixed2 na = radialDisplace(i.uv, _ADropletXY);
				// fixed2 nb = radialDisplace(i.uv, _BDropletXY);
				// fixed2 nc = radialDisplace(i.uv, _CDropletXY);
				// n = max(max(max(nc, nb), na), n);
				fixed4 stepp = step(.001, fixed4(n.xy, n.x * n.y, 0));
				n += i.uv;
				
				fixed4 col = tex2D(_MainTex, n);

				col = saturate(col - stepp * fixed4(i.uv.x, i.uv.y, (i.uv.x + i.uv.y)/2, 0));
				return col;
			}
			ENDCG
		}
	}
}