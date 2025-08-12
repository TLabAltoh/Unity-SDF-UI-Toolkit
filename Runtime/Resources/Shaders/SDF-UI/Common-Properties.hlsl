/**
* Properties used in the sdf-shader
*/

float4 _RectSize;

float _Padding;
float _EulerZ;
float4 _OuterUV;

float _Onion;
float _OnionWidth;

float _GraphicBorder;

float _GraphicGradationAngle;
float _GraphicGradationRadius;
float _GraphicGradationSmooth;
float4 _GraphicGradationColor;
float4 _GraphicGradationLayer;
float4 _GraphicGradationRange;
float4 _GraphicGradationOffset;

// Rainbow gradient properties
float _GraphicUseRainbow;
float _GraphicRainbowSaturation;
float _GraphicRainbowValue;
float _GraphicRainbowHueOffset;

// Outline rainbow property
float _OutlineUseRainbow;

// Shadow rainbow property
float _ShadowUseRainbow;

float _OutlineWidth;
float _OutlineBorder;
float _OutlineInnerBlur;
float _OutlineInnerGaussian;
float _OutlineGradationAngle;
float _OutlineGradationRadius;
float _OutlineGradationSmooth;
float4 _OutlineColor;
float4 _OutlineGradationLayer;
float4 _OutlineGradationColor;
float4 _OutlineGradationRange;
float4 _OutlineGradationOffset;

float _GraphicEffectAngle;
float4 _GraphicEffectColor;
float4 _GraphicEffectOffset;
float _OutlineEffectAngle;
float4 _OutlineEffectColor;
float4 _OutlineEffectOffset;

float _GraphicEffectShinyWidth;
float _GraphicEffectShinyBlur;
float _OutlineEffectShinyWidth;
float _OutlineEffectShinyBlur;

sampler2D _GraphicEffectPatternTex;
float _GraphicEffectPatternRow;
float _GraphicEffectPatternScroll;
float4 _GraphicEffectPatternScale;
float4 _GraphicEffectPatternParams;

sampler2D _OutlineEffectPatternTex;
float _OutlineEffectPatternRow;
float _OutlineEffectPatternScroll;
float4 _OutlineEffectPatternScale;
float4 _OutlineEffectPatternParams;

float _ShadowWidth;
float _ShadowBlur;
float _ShadowBorder;
float _ShadowDilate;
float _ShadowGaussian;
float _ShadowGradationAngle;
float _ShadowGradationRadius;
float _ShadowGradationSmooth;
float4 _ShadowColor;
float4 _ShadowOffset;
float4 _ShadowGradationColor;
float4 _ShadowGradationLayer;
float4 _ShadowGradationRange;
float4 _ShadowGradationOffset;

float4 _ClipRect;
float _UIMaskSoftnessX;
float _UIMaskSoftnessY;

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;
fixed4 _TextureSampleAdd;