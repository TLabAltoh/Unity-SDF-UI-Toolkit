Shader "UI/SDF/Tex/Outline" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
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
        [HideInInspector] _MaxDist("_MaxDist", Float) = 0

        _SDFTex("SDFTex", 2D) = "white" {}

        _Radius("Radius", Float) = 0

        _OnionWidth("Onion Width", Float) = 0

        _ShadowWidth("Shadow Width", Float) = 0
        _ShadowBlur("Shadow Blur", Float) = 0
        _ShadowPower("Shadow Power", Float) = 0
        _ShadowColor("Shadow Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _ShadowOffset("Shadow Offset", Vector) = (0.0, 0.0, 0.0, 1.0)

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

            #pragma shader_feature_local _ SDF_UI_ONION

            #pragma shader_feature_local _ SDF_UI_FASTER_AA
            #pragma shader_feature_local _ SDF_UI_SUPER_SAMPLING_AA
            #pragma shader_feature_local _ SDF_UI_SUBPIXEL_SAMPLING_AA

            #pragma shader_feature_local _ SDF_UI_OUTLINE_INSIDE
            #pragma shader_feature_local _ SDF_UI_OUTLINE_OUTSIDE

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            float _Radius;
            float4 _RectSize;

            float _Padding;
            float _MaxDist;
            float4 _OuterUV;

            float _OnionWidth;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            float _OutlineWidth;
            float4 _OutlineColor;

            sampler2D _SDFTex;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _SDFTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{
                float swapX = i.uv.x;
                float swapY = i.uv.y;
                i.uv.x = 1.0 - swapY;
                i.uv.y = swapX;

                float2 texSample;
                texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
                texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

                half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;
                float2 p = i.uv;
                float2 sp = i.uv - _ShadowOffset.xy;

#ifdef SDF_UI_SUPER_SAMPLING_AA
                float2x2 j = JACOBIAN(p);
                float dist = 0.25 * (
                    - (tex2D(_SDFTex, p + mul(j, float2( 1,  1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2( 1, -1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2(-1,  1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2(-1, -1) * 0.25))).a);

                j = JACOBIAN(sp);
                float sdist = 0.25 * (
                    - (tex2D(_SDFTex, p + mul(j, float2( 1,  1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2( 1, -1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2(-1,  1) * 0.25))).a
                    - (tex2D(_SDFTex, p + mul(j, float2(-1, -1) * 0.25))).a);

                dist = dist * 2.0 + 1.0;
                dist = dist * _MaxDist;

                sdist = sdist * 2.0 + 1.0;
                sdist = sdist * _MaxDist;
#elif SDF_UI_SUBPIXEL_SAMPLING_AA
                float2x2 j = JACOBIAN(p);
                float r = -(tex2D(_SDFTex, p + mul(j, float2(-0.333, 0)))).a;
                float g = -(tex2D(_SDFTex, p)).a;
                float b = -(tex2D(_SDFTex, p + mul(j, float2( 0.333, 0)))).a;
                float4 dist = half4(r, g, b, (r + g + b) / 3.);

                j = JACOBIAN(sp);
                r = -(tex2D(_SDFTex, sp + mul(j, float2(-0.333, 0)))).a;
                g = -(tex2D(_SDFTex, sp)).a;
                b = -(tex2D(_SDFTex, sp + mul(j, float2( 0.333, 0)))).a;
                float4 sdist = half4(r, g, b, (r + g + b) / 3.);

                dist = dist * 2.0 + 1.0;
                dist = dist * _MaxDist;

                sdist = sdist * 2.0 + 1.0;
                sdist = sdist * _MaxDist;
#else
                float dist = -(tex2D(_SDFTex, p)).a;
                dist = dist * 2.0 + 1.0;
                dist = dist * _MaxDist;

                float sdist = -(tex2D(_SDFTex, sp)).a;
                sdist = sdist * 2.0 + 1.0;
                sdist = sdist * _MaxDist;
#endif

#ifdef SDF_UI_ONION
                dist = abs(dist) - _OnionWidth;
                sdist = abs(sdist) - _OnionWidth;
#endif

                dist = round(dist, _Radius);
                sdist = round(sdist, _Radius);

#ifdef SDF_UI_SUBPIXEL_SAMPLING_AA
                float4 delta = fwidth(dist), sdelta = fwidth(sdist);
#elif SDF_UI_SUPER_SAMPLING_AA
                float delta = fwidth(dist), sdelta = fwidth(sdist);
#elif SDF_UI_FASTER_AA
                float offset = -.25; // To offset the pixels of a display, do I need to consider RGBA (divide by 4)?
                dist += offset;
                sdist += offset;

                float delta = fwidth(dist), sdelta = fwidth(sdist);
#else
                float delta = 0, sdelta = 0;
#endif

#ifdef SDF_UI_SUBPIXEL_SAMPLING_AA
                float4 graphicAlpha = 0, outlineAlpha = 0, shadowAlpha = 0;
#else
                float graphicAlpha = 0, outlineAlpha = 0, shadowAlpha = 0;
#endif

#ifdef SDF_UI_OUTLINE_INSIDE
                graphicAlpha = 1 - smoothstep(-_OutlineWidth - delta, -_OutlineWidth, dist);
                outlineAlpha = 1 - smoothstep(-delta, 0, dist);
                shadowAlpha = 1 - smoothstep(_ShadowWidth - _ShadowBlur - sdelta, _ShadowWidth, sdist);
#elif SDF_UI_OUTLINE_OUTSIDE
                outlineAlpha = 1 - smoothstep(_OutlineWidth - delta, _OutlineWidth, dist);
                graphicAlpha = 1 - smoothstep(-delta, 0, dist);
                shadowAlpha = 1 - smoothstep(_OutlineWidth + _ShadowWidth - _ShadowBlur - sdelta, _OutlineWidth + _ShadowWidth, sdist);
#endif

#ifdef SDF_UI_SUBPIXEL_SAMPLING_AA
                half4 lerp0 = lerp(
                    half4(lerp(half3(1, 1, 1), _OutlineColor.rgb, outlineAlpha.rgb), outlineAlpha.a * _OutlineColor.a),   // crop image by outline area
                    color,
                    graphicAlpha    // override with graphic alpha
                );

                half4 effects = lerp(
                    half4(_ShadowColor.rgb, shadowAlpha.a * pow(shadowAlpha.a, _ShadowPower) * _ShadowColor.a),
                    lerp0,
                    lerp0.a // override
                );
#else
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
#endif

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