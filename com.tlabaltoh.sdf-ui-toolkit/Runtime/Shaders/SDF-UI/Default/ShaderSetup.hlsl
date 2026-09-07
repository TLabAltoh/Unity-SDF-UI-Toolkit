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
    half4 mask : TEXCOORD2;
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

    if (_UIVertexColorAlwaysGammaSpace)
    {
        if(!IsGammaSpace())
        {
            v.color.rgb = UIGammaToLinear(v.color.rgb);
        }
    }

    o.color = v.color;

    // This implementation is taken from Unity's build in shader UI-Default.shader.
    // https://unity.com/releases/editor/archive
    float2 pixelSize = o.vertex.w;
    pixelSize /= float2(1, 1) * abs(mul((float2x2)UNITY_MATRIX_P, _ScreenParams.xy));

    float4 clampedRect = clamp(_ClipRect, -2e10, 2e10);
    float2 maskUV = (v.vertex.xy - clampedRect.xy) / (clampedRect.zw - clampedRect.xy);

    o.mask = half4(v.vertex.xy * 2 - clampedRect.xy - clampedRect.zw, 0.25 / (0.25 * half2(_UIMaskSoftnessX, _UIMaskSoftnessY) + abs(pixelSize.xy)));

    return o;
}
