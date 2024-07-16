Shader "UI/SDF/Circle" {
    Properties{
        [HideInInspector] _MainTex("Texture", 2D) = "white" {}
        [HideInInspector] _StencilComp("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask("Stencil Read Mask", Float) = 255
        [HideInInspector] _ColorMask("Color Mask", Float) = 15
        [HideInInspector] _UseUIAlphaClip("Use Alpha Clip", Float) = 0

        _radius("radius", float) = 0.0
        _halfSize("halfSize", Vector) = (0,0,0,0)

        _outlineColor("outlineColor", Color) = (0.0, 0.0, 0.0, 0.0)
        _outlineWidth("outlineWidth", float) = 0.0
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

            float _radius;
            float4 _halfSize;

            float _padding;

            int _onion;
            float _onionWidth;

            float _shadowWidth;
            float _shadowBlur;
            float _shadowPower;
            float4 _shadowColor;

            float _outlineWidth;
            float4 _outlineColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{

                float2 normalizedPadding = float2(_padding / (_halfSize.x * 2), _padding / (_halfSize.y * 2));

                i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

                half4 color = (tex2D(_MainTex, TRANSFORM_TEX(i.uv, _MainTex)) + _TextureSampleAdd) * i.color * _Color;

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                if (color.a <= 0) {
                    return color;
                }

                float2 p = (i.uv - .5) * (_halfSize + _onionWidth) * 2;
                float dist = length(p) - _radius;

                if (_onion) {
                    dist = abs(dist) - _onionWidth;
                }

                float delta = fwidth(dist);

                float graphicAlpha = 1 - smoothstep(-delta, 0, dist);
                float outlineAlpha = (1 - smoothstep(_outlineWidth - delta, _outlineWidth, dist));
                float shadowAlpha = (1 - smoothstep(_shadowWidth - _shadowBlur - delta, _shadowWidth, dist));

                shadowAlpha *= pow(shadowAlpha, _shadowPower) * _shadowColor.a;
                outlineAlpha *= _outlineColor.a;
                graphicAlpha *= color.a;

                half4 effects = lerp(
                    lerp(
                        half4(_shadowColor.rgb, shadowAlpha),
                        half4(_outlineColor.rgb, outlineAlpha),
                        outlineAlpha
                    ),
                    half4(color.rgb, graphicAlpha),
                    graphicAlpha
                );

                return effects;
            }

            ENDCG
        }
    }
}
