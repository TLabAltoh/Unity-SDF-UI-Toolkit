Shader "UI/SDF/Spline/Outline" {
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

        _Radius("Radius", Float) = 0

        _Corner0("Corner 0", Vector) = (0, 0, 0, 0)
        _Corner1("Corner 1", Vector) = (0, 0, 0, 0)
        _Corner2("Corner 2", Vector) = (0, 0, 0, 0)

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

            #pragma shader_feature_local _ SDF_UI_AA_FASTER
            #pragma shader_feature_local _ SDF_UI_AA_SUPER_SAMPLING
            #pragma shader_feature_local _ SDF_UI_AA_SUBPIXEL

            #pragma shader_feature_local _ SDF_UI_OUTLINE_INSIDE
            #pragma shader_feature_local _ SDF_UI_OUTLINE_OUTSIDE

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            float _Radius;
            float4 _RectSize;

            float _Padding;
            float4 _OuterUV;

            float _OnionWidth;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            float _OutlineWidth;
            float4 _OutlineColor;

            float4 _Corner0;
            float4 _Corner1;
            float4 _Corner2;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target {

                float swapX = i.uv.x;
                float swapY = i.uv.y;
                i.uv.x = swapX;
                i.uv.y = 1.0 - swapY;

                float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

                i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

                half4 color = (tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) + _TextureSampleAdd) * _Color;

                float2 uvSample = i.uv;
                uvSample.x = (uvSample.x - _OuterUV.x) / (_OuterUV.z - _OuterUV.x);
                uvSample.y = (uvSample.y - _OuterUV.y) / (_OuterUV.w - _OuterUV.y);

                float2 halfSize = _RectSize * .5;
                float2 p = (i.uv - .5) * (halfSize + _OnionWidth) * 2;
                float2 sp = (i.uv - .5 - _ShadowOffset.xy) * (halfSize + _OnionWidth) * 2;

#ifdef SDF_UI_AA_SUPER_SAMPLING
                float2x2 j = JACOBIAN(p);
                float dist = 0.25 * (
                    sdTriangle(p + mul(j, float2(1,  1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(p + mul(j, float2(1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(p + mul(j, float2(-1,  1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(p + mul(j, float2(-1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy));

                j = JACOBIAN(sp);
                float sdist = 0.25 * (
                    sdTriangle(sp + mul(j, float2(1,  1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(sp + mul(j, float2(1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(sp + mul(j, float2(-1,  1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
                    sdTriangle(sp + mul(j, float2(-1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy));

#elif SDF_UI_AA_SUBPIXEL
                float2x2 j = JACOBIAN(p);
                float r = sdTriangle(p + mul(j, float2(-0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
                float g = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
                float b = sdTriangle(p + mul(j, float2(0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
                float4 dist = half4(r, g, b, (r + g + b) / 3.);

                j = JACOBIAN(sp);
                r = sdTriangle(sp + mul(j, float2(-0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
                g = sdTriangle(sp, _Corner0.xy, _Corner1.xy, _Corner2.xy);
                b = sdTriangle(sp + mul(j, float2(0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
                float4 sdist = half4(r, g, b, (r + g + b) / 3.);
#else
                float dist = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
                float sdist = sdTriangle(sp, _Corner0.xy, _Corner1.xy, _Corner2.xy);
#endif

#ifdef SDF_UI_ONION
                dist = abs(dist) - _OnionWidth;
                sdist = abs(sdist) - _OnionWidth;
#endif

                dist = round(dist, _Radius);
                sdist = round(sdist, _Radius);

#ifdef SDF_UI_AA_SUBPIXEL
                float4 delta = fwidth(dist), sdelta = fwidth(sdist);
#elif defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)
                float delta = fwidth(dist), sdelta = fwidth(sdist);
#else
                float delta = 0, sdelta = 0;
#endif

#ifdef SDF_UI_AA_SUBPIXEL
                float4 graphicAlpha = 0, outlineAlpha = 0, shadowAlpha = 0;
#else
                float graphicAlpha = 0, outlineAlpha = 0, shadowAlpha = 0;
#endif

#ifdef SDF_UI_OUTLINE_INSIDE
                graphicAlpha = 1 - smoothstep(-_OutlineWidth, -_OutlineWidth + delta, dist);
                outlineAlpha = 1 - smoothstep(0, delta, dist);
                shadowAlpha = 1 - smoothstep(_ShadowWidth - _ShadowBlur, _ShadowWidth + sdelta, sdist);
#elif SDF_UI_OUTLINE_OUTSIDE
                outlineAlpha = 1 - smoothstep(_OutlineWidth, _OutlineWidth + delta, dist);
                graphicAlpha = 1 - smoothstep(0, delta, dist);
                shadowAlpha = 1 - smoothstep(_OutlineWidth + _ShadowWidth - _ShadowBlur, _OutlineWidth + _ShadowWidth + sdelta, sdist);
#endif

#ifdef SDF_UI_AA_SUBPIXEL
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
