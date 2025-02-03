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
float aspect = halfSize.y / halfSize.x;

#ifdef SDF_UI_GRAPHIC_EFFECT_PATTERN
float2 graphicPatternPos = float2(i.uv.x, (1. - i.uv.y) * aspect);

float2 graphicPatternUV = graphicPatternPos;
graphicPatternUV -= float2(.5, .5 * aspect);
graphicPatternUV = float2(graphicPatternUV.x * cos(_GraphicEffectAngle) - graphicPatternUV.y * sin(_GraphicEffectAngle), graphicPatternUV.x * sin(_GraphicEffectAngle) + graphicPatternUV.y * cos(_GraphicEffectAngle));
graphicPatternUV += float2(.5, .5 * aspect);
float graphicPatternFade0 = graphicPatternUV.x;
float graphicPatternFade1 = _GraphicEffectOffset.x * _GraphicEffectPatternRow * _GraphicEffectPatternScale.x;
graphicPatternUV -= _GraphicEffectOffset;

graphicPatternPos = graphicPatternUV;
graphicPatternPos *= _GraphicEffectPatternRow;
graphicPatternPos *= _GraphicEffectPatternScale.x;

float2 graphicPatternId = float2(floor(graphicPatternPos.x), floor(graphicPatternPos.y));
float IsGraphicPatternEven = graphicPatternId.x % 2 == 0;
graphicPatternPos = IsGraphicPatternEven * graphicPatternPos + (1. - IsGraphicPatternEven) * (graphicPatternPos - float2(.0, _GraphicEffectPatternParams.y));
graphicPatternId = max(graphicPatternId + graphicPatternFade1, 0);

float2 graphicPatternSample = float2(frac(graphicPatternPos.x), frac(graphicPatternPos.y));

float graphicPatternPower = graphicPatternId.x / _GraphicEffectPatternRow;
float graphicPatternAlpha = graphicPatternPower * _GraphicEffectPatternParams.x + 1;
float graphicPatternScale = graphicPatternPower * _GraphicEffectPatternScale.y + 1;

graphicPatternSample -= .5;
graphicPatternSample *= graphicPatternScale;
graphicPatternSample = clamp(0, 1, graphicPatternSample + .5);
#endif

#ifdef SDF_UI_OUTLINE_EFFECT_PATTERN
float2 outlinePatternPos = float2(i.uv.x, (1. - i.uv.y) * aspect);

float2 outlinePatternUV = outlinePatternPos;
outlinePatternUV -= float2(.5, .5 * aspect);
outlinePatternUV = float2(outlinePatternUV.x * cos(_OutlineEffectAngle) - outlinePatternUV.y * sin(_OutlineEffectAngle), outlinePatternUV.x * sin(_OutlineEffectAngle) + outlinePatternUV.y * cos(_OutlineEffectAngle));
outlinePatternUV += float2(.5, .5 * aspect);
float outlinePatternFade0 = outlinePatternUV.x;
float outlinePatternFade1 = _OutlineEffectOffset.x * _OutlineEffectPatternRow * _OutlineEffectPatternScale.x;
outlinePatternUV -= _OutlineEffectOffset;

outlinePatternPos = outlinePatternUV;
outlinePatternPos *= _OutlineEffectPatternRow;
outlinePatternPos *= _OutlineEffectPatternScale.x;

float2 outlinePatternId = float2(floor(outlinePatternPos.x), floor(outlinePatternPos.y));
float IsOutlinePatternEven = outlinePatternId.x % 2 == 0;
outlinePatternPos = IsOutlinePatternEven * outlinePatternPos + (1. - IsOutlinePatternEven) * (outlinePatternPos - float2(.0, _OutlineEffectPatternParams.y));
outlinePatternId = max(outlinePatternId + outlinePatternFade1, 0);

float2 outlinePatternSample = float2(frac(outlinePatternPos.x), frac(outlinePatternPos.y));

float outlinePatternPower = outlinePatternId.x / _OutlineEffectPatternRow;
float outlinePatternAlpha = outlinePatternPower * _OutlineEffectPatternParams.x + 1;
float outlinePatternScale = outlinePatternPower * _OutlineEffectPatternScale.y + 1;

outlinePatternSample -= .5;
outlinePatternSample *= outlinePatternScale;
outlinePatternSample = clamp(0, 1, outlinePatternSample + .5);
#endif

float2 texSample;
texSample.x = (1. - i.uv.x) * _OuterUV.x + i.uv.x * _OuterUV.z;
texSample.y = (1. - i.uv.y) * _OuterUV.y + i.uv.y * _OuterUV.w;

half4 color = (tex2D(_MainTex, TRANSFORM_TEX(texSample, _MainTex)) + _TextureSampleAdd) * _Color;

half4 effects;