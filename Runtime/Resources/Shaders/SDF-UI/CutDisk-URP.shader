Shader "UI/SDF/CutDisk/Outline/URP" {
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
            #define SDF_UI_STEP_SHADOW 1
            #include "UnityCG.cginc"
            #include "UnityUI.cginc" 
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"
            #include "CutDisk-Properties.hlsl"

            fixed4 frag(v2f i) : SV_Target{

                if (_Height == _Radius) {
                    discard;
                }

                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "ClipByDistance.hlsl"
            }
            #undef SDF_UI_STEP_SHADOW
            #define SDF_UI_STEP_SHADOW 0
            ENDCG
        }
        Pass {

            // This tag is needed for multipass at world space when rendering pipeline is urp
            Tags{"LightMode" = "UniversalForward"}

            CGPROGRAM

            #include "UnityCG.cginc"
            #include "UnityUI.cginc" 
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"
            #include "CutDisk-Properties.hlsl"

            fixed4 frag(v2f i) : SV_Target{

                if (_Height == _Radius) {
                    discard;
                }

                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "ClipByDistance.hlsl"
            }
            ENDCG
        }
    }
}
