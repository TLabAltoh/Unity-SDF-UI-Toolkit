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

float _GraphicShinyWidth;
float _GraphicShinyAngle;
float _GraphicShinyBlur;
float4 _GraphicPatternColor;
float4 _GraphicPatternOffset;

sampler2D _OutlinePatternTexture;
float _OutlineShinyWidth;
float _OutlineShinyAngle;
float _OutlineShinyBlur;
float4 _OutlinePatternColor;
float4 _OutlinePatternOffset;
float4 _OutlinePatternTextureRow;
float4 _OutlinePatternTextureScale;

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