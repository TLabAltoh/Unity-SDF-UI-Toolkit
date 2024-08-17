/**
* Setup step of SDF fragment
*/

#ifdef SDF_UI_TEX
float swapX = i.uv.x;
float swapY = i.uv.y;
i.uv.x = 1.0 - swapY;
i.uv.y = swapX;
#elif defined(SDF_UI_TRIANGLE) || defined(SDF_UI_SPLINE)
float swapX = i.uv.x;
float swapY = i.uv.y;
i.uv.x = swapX;
i.uv.y = 1.0 - swapY;
#endif

#ifndef SDF_UI_TEX
float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

float2 halfSize = _RectSize * .5;
#endif

float2 texSample;
texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;

half4 effects;