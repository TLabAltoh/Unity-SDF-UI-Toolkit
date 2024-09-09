struct appdata {
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float4 color : COLOR;  // set from Image component property
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f {
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float4 color : COLOR;
    float4 worldPosition : TEXCOORD1;
    UNITY_VERTEX_OUTPUT_STEREO
};

v2f vert(appdata v) {
    v2f o;
    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_INITIALIZE_OUTPUT(v2f, o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
    o.worldPosition = v.vertex;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.color = v.color;
    return o;
}

#pragma vertex vert
#pragma fragment frag

#pragma multi_compile_local _ UNITY_UI_CLIP_RECT
#pragma multi_compile_local _ UNITY_UI_ALPHACLIP

#pragma multi_compile_local _ SDF_UI_AA_FASTER SDF_UI_AA_SUPER_SAMPLING SDF_UI_AA_SUBPIXEL

#pragma multi_compile_local SDF_UI_OUTLINE_INSIDE SDF_UI_OUTLINE_OUTSIDE

#pragma multi_compile_local _ SDF_UI_ONION

#pragma multi_compile_local _ SDF_UI_SHADOW_ENABLED