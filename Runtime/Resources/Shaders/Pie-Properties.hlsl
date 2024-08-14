/**
* Properties used in the Pie.shader
*/

float _Theta;
float _Radius;
float4 _RectSize;

float _Padding;
float4 _OuterUV;

float _OnionWidth;

#if SDF_UI_STEP_SHADOW
float _ShadowWidth;
float _ShadowBlur;
float _ShadowPower;
float4 _ShadowColor;
float4 _ShadowOffset;
#else
float _OutlineWidth;
float4 _OutlineColor;

sampler2D _MainTex;
float4 _MainTex_ST;
fixed4 _Color;
fixed4 _TextureSampleAdd;
#endif

float4 _ClipRect;