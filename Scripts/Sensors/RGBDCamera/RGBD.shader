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
Shader "Custom/RGBD"
{

Properties {
    _MainTex ("", 2D) = "white" {}
    _Cutoff ("", Float) = 0.5
    _Color ("", Color) = (1,1,1,1)
}

SubShader {
CGINCLUDE

int _OutputMode;

// remap depth: [0 @ eye .. 1 @ far] => [0 @ near .. 1 @ far]
inline float Linear01FromEyeToLinear01FromNear(float depth01)
{
    float near = _ProjectionParams.y;
    float far = _ProjectionParams.z;
    return (depth01 - near/far) * (1 + near/far);
}

float4 Output(float depth01, float3 normal)
{
    /* see ImageSynthesis.cs
    enum ReplacelementModes {
        DepthCompressed		= 0,
        DepthMultichannel	= 1
    };*/

    if (_OutputMode == 1) // DepthCompressed
    {
        float linearZFromNear = depth01; 
        float k = 1; // compression factor
        return pow(linearZFromNear, k);
    }
    else if (_OutputMode == 2) // DepthMultichannel
    {        
        float linearZFromNear = Linear01FromEyeToLinear01FromNear(depth01);
        float lowBits = frac(linearZFromNear * 256);
        float highBits = linearZFromNear - lowBits / 256;
        return float4(lowBits, highBits, linearZFromNear, 1);
    }

    // unsupported _OutputMode
    return float4(1, 0.5, 0.5, 1);
}
ENDCG

// Support for different RenderTypes
// The following code is based on builtin Internal-DepthNormalsTexture.shader

    Tags { "RenderType"="Opaque" }
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float4 nz : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_base v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag(v2f i) : SV_Target {
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="TransparentCutout" }
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
uniform float4 _MainTex_ST;
v2f vert( appdata_base v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
uniform fixed4 _Color;
fixed4 frag(v2f i) : SV_Target {
    fixed4 texcol = tex2D( _MainTex, i.uv );
    clip( texcol.a*_Color.a - _Cutoff );
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="TreeBark" }
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityBuiltin3xTreeLibrary.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_full v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TreeVertBark(v);
    
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag( v2f i ) : SV_Target {
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="TreeLeaf" }
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "Lighting.cginc"
#include "UnityBuiltin3xTreeLibrary.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert( appdata_full v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TreeVertLeaf(v);
    
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag( v2f i ) : SV_Target {
    half alpha = tex2D(_MainTex, i.uv).a;

    clip (alpha - _Cutoff);
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="TreeOpaque" "DisableBatching"="True" }
    Pass {
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float4 nz : TEXCOORD0;
    UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainAnimateTree(v.vertex, v.color.w);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
fixed4 frag(v2f i) : SV_Target {
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
} 

SubShader {
    Tags { "RenderType"="TreeTransparentCutout" "DisableBatching"="True" }
    Pass {
        Cull Back
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float4 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainAnimateTree(v.vertex, v.color.w);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
    half alpha = tex2D(_MainTex, i.uv).a;

    clip (alpha - _Cutoff);
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
    Pass {
        Cull Front
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
struct appdata {
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    fixed4 color : COLOR;
    float4 texcoord : TEXCOORD0;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};
v2f vert( appdata v ) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainAnimateTree(v.vertex, v.color.w);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy;
    o.nz.xyz = -COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
    fixed4 texcol = tex2D( _MainTex, i.uv );
    clip( texcol.a - _Cutoff );
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }

}

SubShader {
    Tags { "RenderType"="TreeBillboard" }
    Pass {
        Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};
v2f vert (appdata_tree_billboard v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    TerrainBillboardTree(v.vertex, v.texcoord1.xy, v.texcoord.y);
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv.x = v.texcoord.x;
    o.uv.y = v.texcoord.y > 0;
    o.nz.xyz = float3(0,0,1);
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
fixed4 frag(v2f i) : SV_Target {
    fixed4 texcol = tex2D( _MainTex, i.uv );
    clip( texcol.a - 0.001 );
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="GrassBillboard" }
    Pass {
        Cull Off		
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"

struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert (appdata_full v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    WavingGrassBillboardVert (v);
    o.color = v.color;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord.xy;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
    fixed4 texcol = tex2D( _MainTex, i.uv );
    fixed alpha = texcol.a * i.color.a;
    clip( alpha - _Cutoff );
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}

SubShader {
    Tags { "RenderType"="Grass" }
    Pass {
        Cull Off
CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"
#include "TerrainEngine.cginc"
struct v2f {
    float4 pos : SV_POSITION;
    fixed4 color : COLOR;
    float2 uv : TEXCOORD0;
    float4 nz : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert (appdata_full v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    WavingGrassVert (v);
    o.color = v.color;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.texcoord;
    o.nz.xyz = COMPUTE_VIEW_NORMAL;
    o.nz.w = COMPUTE_DEPTH_01;
    return o;
}
uniform sampler2D _MainTex;
uniform fixed _Cutoff;
fixed4 frag(v2f i) : SV_Target {
    fixed4 texcol = tex2D( _MainTex, i.uv );
    fixed alpha = texcol.a * i.color.a;
    clip( alpha - _Cutoff );
    return Output (i.nz.w, i.nz.xyz);
}
ENDCG
    }
}
Fallback Off
}
