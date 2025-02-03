/**
* Properties used in the sdf-shader
*/

float4 _RectSize;

float _Padding;
float4 _OuterUV;

float _Onion;

float _OnionWidth;
float _GraphicBorder;

float _OutlineWidth;
float _OutlineBorder;
float _OutlineInnerBlur;
float _OutlineInnerGaussian;

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
float4 _GraphicEffectPatternScale;
float4 _GraphicEffectPatternParams;
sampler2D _OutlineEffectPatternTex;
float _OutlineEffectPatternRow;
float4 _OutlineEffectPatternScale;
float4 _OutlineEffectPatternParams;

float _ShadowWidth;
float _ShadowBlur;
float _ShadowBorder;
float _ShadowDilate;
float4 _ShadowColor;
float4 _ShadowOffset;
float _ShadowGaussian;

float4 _ClipRect;
float _UIMaskSoftnessX;
float _UIMaskSoftnessY;

sampler2D _MainTex;
float4 _MainTex_ST;
float4 _OutlineColor;
fixed4 _Color;
fixed4 _TextureSampleAdd;