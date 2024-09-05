Shader "Hidden/UI/SDF/Triangle/Outline" {
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
        [HideInInspector] _OuterUV("OuterUV", Vector) = (0, 0, 0, 0)
        [HideInInspector] _ShadowGaussian("ShadowGaussian", Float) = 0

        _Radius("Radius", Float) = 0

        _Corner0("Corner 0", Vector) = (0, 0, 0, 0)
        _Corner1("Corner 1", Vector) = (0, 0, 0, 0)
        _Corner2("Corner 2", Vector) = (0, 0, 0, 0)

        _OnionWidth("Onion Width", Float) = 0

        _ShadowWidth("Shadow Width", Float) = 0
        _ShadowBlur("Shadow Blur", Float) = 0
        _ShadowPower("Shadow Dilate", Float) = 0
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
        Blend One OneMinusSrcAlpha

        Pass {
            CGPROGRAM
#define SDF_UI_TRIANGLE
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            #include "SDFUtils.cginc"
            #include "ShaderSetup.cginc"
            #include "Triangle-Properties.hlsl"

            fixed4 frag(v2f i) : SV_Target {

                #include "FragmentSetup.hlsl"

#define SDF_UI_STEP_SETUP
                #include "SamplingPosition.hlsl"
                #include "Triangle-Distance.hlsl"
                #include "ClipByDistance.hlsl"
#undef SDF_UI_STEP_SETUP

#define SDF_UI_STEP_SHAPE_OUTLINE
                #include "SamplingPosition.hlsl"
                #include "Triangle-Distance.hlsl"
                #include "ClipByDistance.hlsl"
#undef SDF_UI_STEP_SHAPE_OUTLINE

#define SDF_UI_STEP_SHADOW
                #include "SamplingPosition.hlsl"
                #include "Triangle-Distance.hlsl"
                #include "ClipByDistance.hlsl"
#undef SDF_UI_STEP_SHADOW

                #include "FragmentOutput.hlsl"
            }
#undef SDF_UI_TRIANGLE
            ENDCG
        }
    }
}
