// # MIT License

// # Copyright (C) 2018-2021  CrowdBot H2020 European project
// # Inria Rennes Bretagne Atlantique - Rainbow - Julien Pettré

// # Permission is hereby granted, free of charge, to any person obtaining
// # a copy of this software and associated documentation files (the
// # "Software"), to deal in the Software without restriction, including
// # without limitation the rights to use, copy, modify, merge, publish,
// # distribute, sublicense, and/or sell copies of the Software, and to
// # permit persons to whom the Software is furnished to do so, subject
// # to the following conditions:

// # The above copyright notice and this permission notice shall be
// # included in all copies or substantial portions of the Software.

// # THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// # EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// # OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// # NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// # LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// # ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// # CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
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
				// uint depth = round(LinearEyeDepth(f_depth) * 1000000.0) * step(cos(radians(u_MaxAngle)), dot(perp, normal) / length(normal));

				uint depth = round(LinearEyeDepth(f_depth) * 1000000.0);

				// Splitting the depth value into 2 8-bit-part : 
				// The most significant bits for the green channel
				// The least significant bits for the red channel
				uint msb = depth / 256;
				uint lsb = depth % 256;
				// MSB and LSB values need to be scaled down to the [0;1] range

				float3 color = float3((float)lsb / 256.0, (float)msb / 256.0, 0);
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
