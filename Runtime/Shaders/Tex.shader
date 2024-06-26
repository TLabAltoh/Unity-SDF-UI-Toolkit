Shader "UI/SDF/Tex" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}

    // --- Mask support ---
    [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
    [HideInInspector] _Stencil("Stencil ID", Float) = 0
    [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
    [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
    [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
    [HideInInspector] _ColorMask("Color Mask", Float) = 15
    [HideInInspector] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

        // Definition in Properties section is required to Mask works properly
        _SDFTex("SDFTex", 2D) = "white" {}

        _radius("radius", float) = 0.0
        _halfSize("halfSize", Vector) = (0,0,0,0)

        _outlineColor("outlineColor", Color) = (0.0, 0.0, 0.0, 0.0)
        _outlineWidth("outlineWidth", float) = 0.1
            // ---
    }

        SubShader{
            Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

            // --- Mask support ---
            Stencil {
                Ref[_Stencil]
                Comp[_StencilComp]
                Pass[_StencilOp]
                ReadMask[_StencilReadMask]
                WriteMask[_StencilWriteMask]
            }
            Cull Off
            Lighting Off
            ZTest[unity_GUIZTestMode]
            ColorMask[_ColorMask]
            // ---

            Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
            ZWrite Off

            Pass {
                CGPROGRAM

                #include "UnityCG.cginc"
                #include "UnityUI.cginc" 
                #include "SDFUtils.cginc"
                #include "ShaderSetup.cginc"

                #pragma vertex vert
                #pragma fragment frag

                #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
                #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

                float _radius;
                float4 _halfSize;

                int _onion;
                float _onionWidth;

                float _outlineWidth;
                float4 _outlineColor;

                sampler2D _SDFTex;
                sampler2D _MainTex;
                float4 _ClipRect;
                fixed4 _TextureSampleAdd;

                fixed4 frag(v2f i) : SV_Target{
                    float swapX = i.uv.x;
                    float swapY = i.uv.y;
                    i.uv.x = 1.0 - swapY;
                    i.uv.y = swapX;

                    half4 color = (tex2D(_MainTex, i.uv) + _TextureSampleAdd) * i.color;

                    #ifdef UNITY_UI_CLIP_RECT
                    color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                    #endif

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(color.a - 0.001);
                    #endif

                    if (color.a <= 0) {
                        return color;
                    }

                    float dist = -(tex2D(_SDFTex, i.uv)).a;
                    dist = dist * 2.0 + 1.0;
                    dist = dist * _halfSize * 2.0;
                    dist = round(dist, _radius);

                    if (_onion) {
                        dist = abs(dist) - _onionWidth;
                    }

                    float alpha = antialiasedCutoff(dist);

                    #ifdef UNITY_UI_ALPHACLIP
                    clip(alpha - 0.001 - _outlineWidth);
                    #endif

                    if (-dist < _outlineWidth) {
                        i.color = _outlineColor;
                    }

                    return mixAlpha(tex2D(_MainTex, i.uv), i.color, alpha);
                }

                ENDCG
            }
        }
}