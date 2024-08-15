/**
* SDF fragment to determin sampling position
*/

float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;

float2 halfSize = _RectSize * .5;

#if SDF_UI_STEP_SHADOW

#if !defined(SDF_UI_SHADOW_ENABLED)
discard;
#endif

float2 p = (i.uv - .5 - _ShadowOffset.xy) * (halfSize + _OnionWidth) * 2;

#else

float2 texSample;
texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;
float2 p = (i.uv - .5) * (halfSize + _OnionWidth) * 2;

#endif