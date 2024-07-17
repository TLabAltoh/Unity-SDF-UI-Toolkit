Shader "UI/SDF/Pie" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

        [HideInInspector] _HalfSize("HalfSize", Vector) = (0, 0, 0, 0)
        [HideInInspector] _Padding("Padding", Float) = 0

        _Radius("Radius", Float) = 0
        _Theta("Theta", Float) = 0

        _Onion("Onion", Float) = 0
        _OnionWidth("Onion Width", Float) = 0

        _ShadowWidth("Shadow Width", Float) = 0
        _ShadowBlur("Shadow Blur", Float) = 0
        _ShadowPower("Shadow Power", Float) = 0
        _ShadowColor("Shadow Color", Color) = (0.0, 0.0, 0.0, 1.0)

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

            float _Theta;
            float _Radius;
            float4 _HalfSize;

            float _Padding;

            int _Onion;
            float _OnionWidth;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;

            float _OutlineWidth;
            float4 _OutlineColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{

                float2 normalizedPadding = float2(_Padding / (_HalfSize.x * 2), _Padding / (_HalfSize.y * 2));

                i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

                half4 color = (tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) + _TextureSampleAdd) * i.color * _Color;

                float2 p = (i.uv - .5) * (_HalfSize + _OnionWidth) * 2;
                float dist = _Theta >= 3.14 ? length(p) - _Radius : sdPie(p, float2(sin(_Theta), cos(_Theta)), _Radius);

                if (_Onion) {
                    dist = abs(dist) - _OnionWidth;
                }

                float delta = fwidth(dist);

                float graphicAlpha = 1 - smoothstep(-delta, 0, dist);
                float outlineAlpha = (1 - smoothstep(_OutlineWidth - delta, _OutlineWidth, dist));
                float shadowAlpha = (1 - smoothstep(_ShadowWidth - _ShadowBlur - delta, _ShadowWidth, dist));

                shadowAlpha *= pow(shadowAlpha, _ShadowPower) * _ShadowColor.a;
                outlineAlpha *= _OutlineColor.a;
                graphicAlpha *= color.a;

                half4 effects = lerp(
                    lerp(
                        half4(_ShadowColor.rgb, shadowAlpha),
                        half4(_OutlineColor.rgb, outlineAlpha),
                        outlineAlpha
                    ),
                    half4(color.rgb, graphicAlpha),
                    graphicAlpha
                );

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
