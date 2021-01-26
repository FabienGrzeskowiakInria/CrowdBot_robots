Shader "Custom/Depth"
{
	Properties{
		_MainTex("Texture", 2D) = "white" { }
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Tags
		{
			"RenderType"="Opaque"
		}

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "noiseSimplex.cginc"


			sampler2D _CameraDepthNormalsTexture;
			sampler2D _MainTex;
			float u_MaxAngle;

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata_base v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex); 
				o.uv = v.texcoord;
				return o;
			}
			

			float3 frag (v2f i) : SV_TARGET
			{
				float f_depth;
				float3 normal;
				DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, i.uv), f_depth, normal);
				float3 perp = float3(0.0, 0.0, 1.0);

				// Depths where normal angle is above specified u_MaxAngle are discarded (set to zero) 
				uint depth = round(LinearEyeDepth(f_depth) * 1000000.0) * step(cos(radians(u_MaxAngle)), dot(perp, normal) / length(normal));

				// Splitting the depth value into 2 8-bit-part : 
				// The most significant bits for the green channel
				// The least significant bits for the red channel
				uint msb = depth / 256;
				uint lsb = depth % 256;
				// MSB and LSB values need to be scaled down to the [0;1] range

				float3 color = float3((float)lsb / 256.0, (float)msb / 256.0, (float)msb/256.0);
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
