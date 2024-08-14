Shader "UI/SDF/CutDisk/Outline" {
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
        _Height("Height", Float) = 0

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

            float _Height;
            float _Radius;
            float4 _RectSize;

            float _Padding;
            float _Rotation;
            float4 _OuterUV;

            float _OnionWidth;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            float _OutlineWidth;
            float4 _OutlineColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{

                if (_Height == _Radius) {
                    discard;
                }

                #include "SamplingPosition.hlsl"

                #ifdef SDF_UI_AA_SUPER_SAMPLING
                    float2x2 j = JACOBIAN(p);
                    float dist = 0.25 * (
                    sdCutDisk(p + mul(j, float2(1,  1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(1, -1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(-1,  1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(-1, -1) * 0.25), _Radius, _Height));
                #elif SDF_UI_AA_SUBPIXEL
                    float2x2 j = JACOBIAN(p);
                    float r = sdCutDisk(p + mul(j, float2(-0.333, 0)), _Radius, _Height);
                    float g = sdCutDisk(p, _Radius, _Height);
                    float b = sdCutDisk(p + mul(j, float2(0.333, 0)), _Radius, _Height);
                    float4 dist = half4(r, g, b, (r + g + b) / 3.);
                #else
                    float dist = sdCutDisk(p, _Radius, _Height);
                #endif

                #ifdef SDF_UI_ONION
                    dist = abs(dist) - _OnionWidth;
                #endif

                #include "ClipByDistance.hlsl"
            }
            ENDCG
        }
        Pass {
            CGPROGRAM

            #include "UnityCG.cginc"
            #include "UnityUI.cginc" 
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"

            float _Height;
            float _Radius;
            float4 _RectSize;

            float _Padding;
            float _Rotation;
            float4 _OuterUV;

            float _OnionWidth;

            float _ShadowWidth;
            float _ShadowBlur;
            float _ShadowPower;
            float4 _ShadowColor;
            float4 _ShadowOffset;

            float _OutlineWidth;
            float4 _OutlineColor;

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float4 _ClipRect;
            fixed4 _TextureSampleAdd;

            fixed4 frag(v2f i) : SV_Target{

                if (_Height == _Radius) {
                    discard;
                }

                #include "SamplingPosition.hlsl"

                #ifdef SDF_UI_AA_SUPER_SAMPLING
                    float2x2 j = JACOBIAN(p);
                    float dist = 0.25 * (
                    sdCutDisk(p + mul(j, float2(1,  1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(1, -1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(-1,  1) * 0.25), _Radius, _Height) +
                    sdCutDisk(p + mul(j, float2(-1, -1) * 0.25), _Radius, _Height));
                #elif SDF_UI_AA_SUBPIXEL
                    float2x2 j = JACOBIAN(p);
                    float r = sdCutDisk(p + mul(j, float2(-0.333, 0)), _Radius, _Height);
                    float g = sdCutDisk(p, _Radius, _Height);
                    float b = sdCutDisk(p + mul(j, float2(0.333, 0)), _Radius, _Height);
                    float4 dist = half4(r, g, b, (r + g + b) / 3.);
                #else
                    float dist = sdCutDisk(p, _Radius, _Height);
                #endif

                #ifdef SDF_UI_ONION
                    dist = abs(dist) - _OnionWidth;
                #endif

                #include "ClipByDistance.hlsl"
            }
            ENDCG
        }
    }
}
