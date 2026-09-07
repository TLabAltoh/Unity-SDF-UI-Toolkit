/**
* Setup step of SDF fragment
*/

float2 uv;
#if defined(SDF_UI_TRIANGLE) || defined(SDF_UI_SPLINE)
uv.x = i.uv.x;
uv.y = 1.0 - i.uv.y;
#else
uv = i.uv;
#endif

#ifndef SDF_UI_TEX
float2 normalizedPadding = float2(_Padding / _RectSize.x, _Padding / _RectSize.y);

i.uv = i.uv * (1 + normalizedPadding * 2) - normalizedPadding;
uv = uv * (1 + normalizedPadding * 2) - normalizedPadding;
#endif

float2 halfSize = _RectSize * .5;
float aspect = halfSize.y / halfSize.x, hminSize = min(halfSize.x, halfSize.y), minSize = 2 * hminSize;

#ifdef SDF_UI_GRAPHIC_EFFECT_PATTERN
float2 graphicPatternPos = float2(i.uv.x, (1. - i.uv.y) * aspect);

float2 graphicPatternUV = graphicPatternPos;
graphicPatternUV -= float2(.5, .5 * aspect);
graphicPatternUV = rotate(graphicPatternUV, _GraphicEffectAngle - _EulerZ);
graphicPatternUV += float2(.5, .5 * aspect);
graphicPatternUV -= _GraphicEffectOffset;
float graphicPatternFade0 = graphicPatternUV.x;
float graphicPatternFade1 = _GraphicEffectPatternScroll * _GraphicEffectPatternRow * _GraphicEffectPatternScale.x;
graphicPatternUV.x -= _GraphicEffectPatternScroll;

graphicPatternPos = graphicPatternUV;
graphicPatternPos *= _GraphicEffectPatternRow;
graphicPatternPos *= _GraphicEffectPatternScale.x;

float2 graphicPatternId = float2(floor(graphicPatternPos.x), floor(graphicPatternPos.y));
float IsGraphicPatternEven = (fmod(graphicPatternId.x, 2.0) == 0.0) ? 1.0 : 0.0;
graphicPatternPos = IsGraphicPatternEven * graphicPatternPos + (1. - IsGraphicPatternEven) * (graphicPatternPos - float2(.0, _GraphicEffectPatternParams.y));
graphicPatternId = max(graphicPatternId + graphicPatternFade1, 0);

float2 graphicPatternSample = float2(frac(graphicPatternPos.x), frac(graphicPatternPos.y));

float graphicPatternPower = graphicPatternId.x / _GraphicEffectPatternRow;
float graphicPatternAlpha = graphicPatternPower * _GraphicEffectPatternParams.x + 1;
float graphicPatternScale = graphicPatternPower * _GraphicEffectPatternScale.y + 1;

graphicPatternSample -= .5;
graphicPatternSample *= graphicPatternScale;
graphicPatternSample = clamp(graphicPatternSample + .5, 0, 1);
#endif

#ifdef SDF_UI_OUTLINE_EFFECT_PATTERN
float2 outlinePatternPos = float2(i.uv.x, (1. - i.uv.y) * aspect);

float2 outlinePatternUV = outlinePatternPos;
outlinePatternUV -= float2(.5, .5 * aspect);
outlinePatternUV = rotate(outlinePatternUV, _OutlineEffectAngle - _EulerZ);
outlinePatternUV += float2(.5, .5 * aspect);
outlinePatternUV -= _OutlineEffectOffset;
float outlinePatternFade0 = outlinePatternUV.x;
float outlinePatternFade1 = _OutlineEffectPatternScroll * _OutlineEffectPatternRow * _OutlineEffectPatternScale.x;
outlinePatternUV.x -= _OutlineEffectPatternScroll;

outlinePatternPos = outlinePatternUV;
outlinePatternPos *= _OutlineEffectPatternRow;
outlinePatternPos *= _OutlineEffectPatternScale.x;

float2 outlinePatternId = float2(floor(outlinePatternPos.x), floor(outlinePatternPos.y));
float IsOutlinePatternEven = (fmod(outlinePatternId.x, 2.0) == 0.0) ? 1.0 : 0.0;
outlinePatternPos = IsOutlinePatternEven * outlinePatternPos + (1. - IsOutlinePatternEven) * (outlinePatternPos - float2(.0, _OutlineEffectPatternParams.y));
outlinePatternId = max(outlinePatternId + outlinePatternFade1, 0);

float2 outlinePatternSample = float2(frac(outlinePatternPos.x), frac(outlinePatternPos.y));

float outlinePatternPower = outlinePatternId.x / _OutlineEffectPatternRow;
float outlinePatternAlpha = outlinePatternPower * _OutlineEffectPatternParams.x + 1;
float outlinePatternScale = outlinePatternPower * _OutlineEffectPatternScale.y + 1;

outlinePatternSample -= .5;
outlinePatternSample *= outlinePatternScale;
outlinePatternSample = clamp(outlinePatternSample + .5, 0, 1);
#endif

float2 texSample;
texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

half4 texcr = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd);
half4 color = texcr * _Color;
_GraphicGradationColor *= texcr;

half4 effects;