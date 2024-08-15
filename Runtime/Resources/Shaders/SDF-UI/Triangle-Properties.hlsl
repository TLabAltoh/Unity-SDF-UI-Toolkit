/**
* Properties used in the Triangle.shader
*/

float _Radius;
float4 _RectSize;

float _Padding;
float4 _OuterUV;

float4 _Corner0;
float4 _Corner1;
float4 _Corner2;

float _OnionWidth;
float _OutlineWidth;

#if SDF_UI_STEP_SHADOW
float _ShadowWidth;
float _ShadowBlur;
float _ShadowPower;
float4 _ShadowColor;
float4 _ShadowOffset;
#else
sampler2D _MainTex;
float4 _MainTex_ST;
float4 _OutlineColor;
fixed4 _Color;
fixed4 _TextureSampleAdd;
#endif

float4 _ClipRect;