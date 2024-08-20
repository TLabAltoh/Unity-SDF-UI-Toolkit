/**
* Setup step of SDF fragment
*/

float2 uv;
#ifdef SDF_UI_TEX
uv = i.uv;
#elif defined(SDF_UI_TRIANGLE) || defined(SDF_UI_SPLINE)
uv.x = i.uv.x;
uv.y = 1.0 - i.uv.y;
#else
uv = i.uv;
#endif

#ifndef SDF_UI_TEX
float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;
uv = uv * (1 + normalizedPadding * 2) - normalizedPadding;

float2 halfSize = _RectSize * .5;
#endif

float2 texSample;
texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;

half4 effects;