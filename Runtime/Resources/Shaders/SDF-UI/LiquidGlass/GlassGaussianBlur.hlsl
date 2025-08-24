/**
* 2-pass-blur
*/

#if 0

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

struct appdata {
    float4 vertex : POSITION;
};

struct v2f {
    float4 vertex : SV_POSITION;
    float4 uv : TEXCOORD0;
};

sampler2D _GrabTexture;
float4 _GrabTexture_TexelSize;

v2f vert(appdata v) {
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = ComputeGrabScreenPos(o.vertex);
    return o;
}

fixed4 frag(v2f i) : SV_Target {
      float2 uv = i.uv.xy / i.uv.w;
      float blur=18, weight_total=0;
      fixed4 col = fixed4(0,0,0,0);
      
      blur = max(1, blur);
      
      [loop]
      for (float i = -blur; i <= blur; i++) {
        float distance_normalized = abs(i / blur);
        float weight = exp(-2.5 * distance_normalized * distance_normalized);
        weight_total += weight;
        col += tex2D(_GrabTexture, uv + GLASS_BLUR_OFFSET * _GrabTexture_TexelSize.xy) * weight;
      }
      return col / weight_total;
}

#endif
