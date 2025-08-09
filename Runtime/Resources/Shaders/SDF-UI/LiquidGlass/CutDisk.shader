Shader "Hidden/UI/SDF/CutDisk/LiquidGlass/Outline" {
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
        [HideInInspector] _EulerZ("EulerZ", Float) = 0
        [HideInInspector] _OuterUV("OuterUV", Vector) = (0, 0, 0, 0)

        [HideInInspector] _GraphicBorder("Graphic Border", Float) = 0
        [HideInInspector] _OutlineBorder("Outline Border", Float) = 0
        [HideInInspector] _ShadowBorder("Shadow Border", Float) = 0

        [HideInInspector] _Gamma("Gamma", Float) = 0.455 // 1.0 / 2.2

        _LiquidGlassThickness("LiquidGlassThickness", Float) = 0
        _LiquidGlassIndex("LiquidGlassIndex", Float) = 0
        _LiquidGlassBaseHeight("LiquidGlassBaseHeight", Float) = 0

        _Radius("Radius", Float) = 0
        _Height("Height", Float) = 0

        _Onion("Onion", Float) = 0
        _OnionWidth("Onion Width", Float) = 0

        _GraphicGradationAngle("Graphic Gradation Angle", Float) = 0
        _GraphicGradationSmooth("Graphic Gradation Smooth", Float) = 0
        _GraphicGradationRange("Graphic Gradation Range", Vector) = (0.0, 0.0, 0.0, 1.0)
        _GraphicGradationLayer("Graphic Gradation Layer", Vector) = (0.0, 0.0, 0.0, 1.0)
        _GraphicGradationOffset("Graphic Gradation Offset", Vector) = (0.0, 0.0, 0.0, 1.0)
        _GraphicGradationColor("Graphic Gradation Color", Color) = (0.0, 0.0, 0.0, 1.0)

        _OutlineWidth("Outline Width", Float) = 0
        _OutlineBorder("Outline Border", Float) = 0
        _OutlineInnerBlur("Outline Inner Blur", Float) = 0
        _OutlineInnerGaussian("Outline Inner Gaussian", Float) = 0
        _OutlineGradationAngle("Outline Gradation Angle", Float) = 0
        _OutlineGradationSmooth("Outline Gradation Smooth", Float) = 0
        _OutlineGradationRange("Outline Gradation Range", Vector) = (0.0, 0.0, 0.0, 1.0)
        _OutlineGradationLayer("Outline Gradation Layer", Vector) = (0.0, 0.0, 0.0, 1.0)
        _OutlineGradationOffset("Outline Gradation Offset", Vector) = (0.0, 0.0, 0.0, 1.0)
        [HDR] _OutlineColor("Outline Color", Color) = (0.0, 0.0, 0.0, 1.0)
        [HDR] _OutlineGradationColor("Outline Gradation Color", Color) = (0.0, 0.0, 0.0, 1.0)

        _ShadowWidth("Shadow Width", Float) = 0
        _ShadowBlur("Shadow Blur", Float) = 0
        _ShadowPower("Shadow Dilate", Float) = 0
        _ShadowGaussian("Shadow Gaussian", Float) = 0
        _ShadowOffset("Shadow Offset", Vector) = (0.0, 0.0, 0.0, 1.0)
        _ShadowGradationAngle("Shadow Gradation Angle", Float) = 0
        _ShadowGradationSmooth("Shadow Gradation Smooth", Float) = 0
        _ShadowGradationRange("Shadow Gradation Range", Vector) = (0.0, 0.0, 0.0, 1.0)
        _ShadowGradationLayer("Shadow Gradation Layer", Vector) = (0.0, 0.0, 0.0, 1.0)
        _ShadowGradationOffset("Shadow Gradation Offset", Vector) = (0.0, 0.0, 0.0, 1.0)
        [HDR] _ShadowColor("Shadow Color", Color) = (0.0, 0.0, 0.0, 1.0)
        [HDR] _ShadowGradationColor("Shadow Gradation Color", Color) = (0.0, 0.0, 0.0, 1.0)

        _GraphicEffectAngle("Graphic Effect Angle", Float) = 0
        [HDR] _GraphicEffectColor("Graphic Effect Color", Vector) = (0.0, 0.0, 0.0, 1.0)
        _GraphicEffectOffset("Graphic Effect Offset", Vector) = (0.0, 0.0, 0.0, 1.0)
        _OutlineEffectAngle("Outline Effect Angle", Float) = 0
        [HDR] _OutlineEffectColor("Outline Effect Color", Vector) = (0.0, 0.0, 0.0, 1.0)
        _OutlineEffectOffset("Outline Effect Offset", Vector) = (0.0, 0.0, 0.0, 1.0)

        _GraphicEffectShinyWidth("Graphic Effect Shiny Width", Float) = 0
        _GraphicEffectShinyBlur("Graphic Effect Shiny Blur", Float) = 0
        _OutlineEffectShinyWidth("Outline Effect Shiny Width", Float) = 0
        _OutlineEffectShinyBlur("Outline Effect Shiny Blur", Float) = 0

        _OutlineEffectPatternTex("Outline Effect Pattern Tex", 2D) = "white" {}
        _OutlineEffectPatternRow("Outline Effect Pattern Row", Float) = 0
        _OutlineEffectPatternScale("Outline Effect Pattern Scale", Vector) = (0.0, 0.0, 0.0, 1.0)
        _OutlineEffectPatternParams("Outline Effect Pattern Params", Vector) = (0.0, 0.0, 0.0, 1.0)
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            CGPROGRAM
#define SDF_UI_CUT_DISK
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #include "CutDisk-Properties.hlsl"
            #include "SDFUtils.cginc"
            #include "ShaderSetup.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma multi_compile_local _ SDF_UI_AA
            #pragma multi_compile_local _ SDF_UI_SHADOW

            fixed4 frag(v2f i) : SV_Target {

                if (_Height == _Radius) {
                    discard;
                }

                #include "FragmentSetup.hlsl"

#define SDF_UI_STEP_SETUP
                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "Pattern-Distance.hlsl"
                #include "ClipByDistance.hlsl"
#undef SDF_UI_STEP_SETUP

#define SDF_UI_STEP_SHADOW
                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "ClipByDistance.hlsl"
                #include "FragmentOutput.hlsl"
#undef SDF_UI_STEP_SHADOW
            }
#undef SDF_UI_CUT_DISK
            ENDCG
        }
        GrabPass { }
        Pass {
            CGPROGRAM
#define SDF_UI_CUT_DISK
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #include "CutDisk-Properties.hlsl"
            #include "SDFUtils.cginc"
            #include "ShaderSetup.hlsl"

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            #pragma multi_compile_local _ SDF_UI_AA

            #pragma multi_compile_local _ SDF_UI_OUTLINE_EFFECT_SHINY SDF_UI_OUTLINE_EFFECT_PATTERN
            #pragma multi_compile_local _ SDF_UI_GRAPHIC_EFFECT_SHINY SDF_UI_GRAPHIC_EFFECT_PATTERN

            fixed4 frag(v2f i) : SV_Target {

                if (_Height == _Radius) {
                    discard;
                }

                #include "FragmentSetup.hlsl"

#define SDF_UI_STEP_SETUP
                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "Pattern-Distance.hlsl"
                #include "ClipByDistance.hlsl"
#undef SDF_UI_STEP_SETUP

#define SDF_UI_STEP_SHAPE_AND_OUTLINE
                #include "SamplingPosition.hlsl"
                #include "CutDisk-Distance.hlsl"
                #include "Pattern-Distance.hlsl"
                #include "ClipByDistance.hlsl"
                #include "FragmentOutput.hlsl"
#undef SDF_UI_STEP_SHAPE_AND_OUTLINE
            }
#undef SDF_UI_CUT_DISK
            ENDCG
        }
    }
}
