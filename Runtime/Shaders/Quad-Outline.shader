Shader "UI/SDF/Quad/Outline" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _Color("Tint", Color) = (1,1,1,1)
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

        [HideInInspector] _RectSize("RectSize", Vector) = (0, 0, 0, 0)
        [HideInInspector] _Padding("Padding", Float) = 0
        [HideInInspector] _OuterUV("_OuterUV", Vector) = (0, 0, 0, 0)

        _Radius("Radius", Vector) = (0, 0, 0, 0)

        _Onion("Onion", Int) = 0
        _OnionWidth("Onion Width", Float) = 0

        _Antialiasing("Antialiasing", Int) = 0

        _ShadowWidth("Shadow Width", Float) = 0
        _ShadowBlur("Shadow Blur", Float) = 0
        _ShadowPower("Shadow Power", Float) = 0
        _ShadowColor("Shadow Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _ShadowOffset("Shadow Offset", Vector) = (0.0, 0.0, 0.0, 1.0)

        _OutlineType("Outline Type", Int) = 0
        _OutlineWidth("Outline Width", Float) = 0
        _OutlineColor("Outline Color", Color) = (0.0, 0.0, 0.0, 1.0)
    }

    SubShader{
        Tags {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil {
            Ref[_Stencil]
            Comp[_StencilComp]
            Pass[_StencilOp]
            ReadMask[_StencilReadMask]
            WriteMask[_StencilWriteMask]
        }
        Cull Off
        ZWrite Off
        Lighting Off
        ZTest[unity_GUIZTestMode]
        ColorMask[_ColorMask]
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha

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

            float4 _Radius;
            float4 _RectSize;

            float _Padding;
            float4 _OuterUV;

            int _Onion;
            float _OnionWidth;

            int _Antialiasing;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            int _OutlineType;
            float _OutlineWidth;
            float4 _OutlineColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{

                float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

                i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

                float2 texSample;
                texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
                texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

                half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;

                float2 halfSize = _RectSize * .5;
                float2 p = (i.uv - .5) * (halfSize + _OnionWidth) * 2;
                float2 sp = (i.uv - .5 - _ShadowOffset.xy) * (halfSize + _OnionWidth) * 2;

                float dist = sdRoundedBox(p, halfSize, _Radius);
                float sdist = sdRoundedBox(sp, halfSize, _Radius);

                if (_Onion) {
                    dist = abs(dist) - _OnionWidth;
                    sdist = abs(sdist) - _OnionWidth;
                }

                float delta = 0, sdelta = 0;

                if (_Antialiasing) {
                    float offset = -.25; // To offset the pixels of a display, do I need to consider RGBA (divide by 4)?
                    dist += offset;
                    sdist += offset;

                    delta = fwidth(dist);
                    sdelta = fwidth(sdist);
                }

                float graphicAlpha, outlineAlpha, shadowAlpha;
                if (_OutlineType == 0) {    // Inside
                    graphicAlpha = 1 - smoothstep(-_OutlineWidth - delta, -_OutlineWidth, dist);
                    outlineAlpha = 1 - smoothstep(-delta, 0, dist);
                    shadowAlpha = 1 - smoothstep(_ShadowWidth - _ShadowBlur - sdelta, _ShadowWidth, sdist);
                }
                else {  // Outside
                    outlineAlpha = 1 - smoothstep(_OutlineWidth - delta, _OutlineWidth, dist);
                    graphicAlpha = 1 - smoothstep(-delta, 0, dist);
                    shadowAlpha = 1 - smoothstep(_OutlineWidth + _ShadowWidth - _ShadowBlur - sdelta, _OutlineWidth + _ShadowWidth, sdist);
                }

                half4 lerp0 = lerp(
                    half4(_OutlineColor.rgb, outlineAlpha * _OutlineColor.a),   // crop image by outline area
                    half4(color.rgb, color.a),
                    graphicAlpha    // override with graphic alpha
                );

                half4 effects = lerp(
                    half4(_ShadowColor.rgb, shadowAlpha * pow(shadowAlpha, _ShadowPower) * _ShadowColor.a),
                    lerp0,
                    lerp0.a // override
                );

                effects *= i.color;

                half t = effects.a - 0.001;

#ifdef UNITY_UI_CLIP_RECT
                effects.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
                clip(effects.a - 0.001 > 0.0 ? 1 : -1);
#endif

                return effects;
            }

            ENDCG
        }
    }
}
