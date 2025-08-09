/**
* SDF fragment after determined distance
*/

#define PI 3.14

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float delta = 0, softBorder0 = 0, softBorder1 = 0, softAlpha0 = 0, softAlpha1 = 0;

#endif // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHADOW
#ifdef SDF_UI_SHADOW

#ifdef SDF_UI_AA
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

softBorder0 = _ShadowBorder - _ShadowBlur - delta;
softBorder1 = _ShadowBorder + delta;

#ifdef SDF_UI_AA
softAlpha0 = 1 - saturaterange(softBorder0, softBorder1, dist);
#else
softAlpha0 = 1 - (dist >= softBorder0);
#endif

softAlpha1 = 1 - smoothstep(softBorder0, softBorder1, dist);

/**
*
* I added gradation (linear, radial, conic) for graphic, outline, shadow. I calculate both linear gradation and
* conical gradation and selecting one with flag parameter to avoid increasing number of shader variants
* cause of adding shader keyword (if shader keyword is used, it increases shader variant x64 (= 4 x 4 x 4)
* ((None, Linear, Radial, Conical) , (Graphic, Outline, Shadow))). Maybe this way increases performance cost
* (not measured yet) and unsuitable for switching many layers.
*
*/

float shadowAlpha = lerp(softAlpha0, softAlpha1, _ShadowGaussian);
#if 1
float shadowGradationAngle = PI * _ShadowGradationAngle;
#ifdef SDF_UI_TEX
float shadowGradationSmooth = _ShadowGradationSmooth;
float shadowGradationRadius = _ShadowGradationRadius;
float2 shadowGradationOffset = _ShadowGradationOffset + float2(.5, .5);
#else
float shadowGradationSmooth = _ShadowGradationSmooth * minSize;
float shadowGradationRadius = _ShadowGradationRadius * minSize;
float2 shadowGradationOffset = _ShadowGradationOffset * _RectSize.xy;
#endif
float2 shadowGradationPosition = rotate(pRotated - shadowGradationOffset, shadowGradationAngle);
float4 shadowMixedColor0 = linearGradation(shadowGradationPosition, shadowGradationSmooth, _ShadowColor, _ShadowGradationColor);
float4 shadowMixedColor1 = radialGradation(shadowGradationPosition, shadowGradationRadius, shadowGradationSmooth, _ShadowColor, _ShadowGradationColor);
float4 shadowMixedColor2 = conicalGradation(shadowGradationPosition, shadowGradationSmooth, _ShadowGradationRange, _ShadowColor, _ShadowGradationColor);

// Rainbow gradient implementation for shadow
float4 shadowRainbowMixedColor0 = rainbowLinearGradation(shadowGradationPosition, shadowGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 shadowRainbowMixedColor1 = rainbowRadialGradation(shadowGradationPosition, shadowGradationRadius, shadowGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 shadowRainbowMixedColor2 = rainbowConicalGradation(shadowGradationPosition, shadowGradationSmooth, _ShadowGradationRange, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);

// Apply alpha from shadow gradation color to rainbow colors
shadowRainbowMixedColor0.a = _ShadowGradationColor.a;
shadowRainbowMixedColor1.a = _ShadowGradationColor.a;
shadowRainbowMixedColor2.a = _ShadowGradationColor.a;

// Select between normal gradation and rainbow gradation for shadow
float4 shadowNormalGradation = select(_ShadowGradationLayer, _ShadowColor, shadowMixedColor0, shadowMixedColor1, shadowMixedColor2);
float4 shadowRainbowGradation = select(_ShadowGradationLayer, _ShadowColor, shadowRainbowMixedColor0, shadowRainbowMixedColor1, shadowRainbowMixedColor2);
float4 shadowColor = lerp(shadowNormalGradation, shadowRainbowGradation, _ShadowUseRainbow);
#else
float4 shadowColor = _ShadowColor;
#endif

/**
*
*
*
*/

{
	half4 layer0 = shadowColor;
	layer0.a *= shadowAlpha;
	layer0.a *= (1. - effects.a);
	layer0.rgb *= layer0.a;
	effects = effects + layer0;
}

#endif	// SDF_UI_SHADOW
#endif	// SDF_UI_STEP_SHADOW

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE

#ifdef SDF_UI_AA
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

float graphicAlpha = 0, outlineAlpha = 0;

softBorder0 = _GraphicBorder - _OutlineInnerBlur - delta;
softBorder1 = _GraphicBorder + delta;

#ifdef SDF_UI_AA
outlineAlpha = 1 - saturaterange(_OutlineBorder - delta, _OutlineBorder + delta, dist);
#else
outlineAlpha = 1 - (dist >= _OutlineBorder);
#endif

#ifdef SDF_UI_AA
softAlpha0 = 1 - saturaterange(softBorder0, softBorder1, dist);
#else
softAlpha0 = 1 - (dist >= softBorder0);
#endif
softAlpha1 = 1 - smoothstep(softBorder0, softBorder1, dist);

graphicAlpha = lerp(softAlpha0, softAlpha1, _OutlineInnerGaussian);

/**
*
*
*
*/

#if 1
float outlineGradationAngle = PI * _OutlineGradationAngle;
#ifdef SDF_UI_TEX
float outlineGradationSmooth = _OutlineGradationSmooth;
float outlineGradationRadius = _OutlineGradationRadius;
float2 outlineGradationOffset = _OutlineGradationOffset.xy + float2(.5, .5);
#else
float outlineGradationSmooth = _OutlineGradationSmooth * minSize;
float outlineGradationRadius = _OutlineGradationRadius * minSize;
float2 outlineGradationOffset = _OutlineGradationOffset.xy * _RectSize.xy;
#endif
float2 outlineGradationPosition = rotate(pRotated - outlineGradationOffset, outlineGradationAngle);
float4 outlineMixedColor0 = linearGradation(outlineGradationPosition, outlineGradationSmooth, _OutlineColor, _OutlineGradationColor);
float4 outlineMixedColor1 = radialGradation(outlineGradationPosition, outlineGradationRadius, outlineGradationSmooth, _OutlineColor, _OutlineGradationColor);
float4 outlineMixedColor2 = conicalGradation(outlineGradationPosition, outlineGradationSmooth, _OutlineGradationRange, _OutlineColor, _OutlineGradationColor);

// Rainbow gradient implementation for outline
float4 outlineRainbowMixedColor0 = rainbowLinearGradation(outlineGradationPosition, outlineGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 outlineRainbowMixedColor1 = rainbowRadialGradation(outlineGradationPosition, outlineGradationRadius, outlineGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 outlineRainbowMixedColor2 = rainbowConicalGradation(outlineGradationPosition, outlineGradationSmooth, _OutlineGradationRange, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);

// Apply alpha from outline gradation color to rainbow colors
outlineRainbowMixedColor0.a = _OutlineGradationColor.a;
outlineRainbowMixedColor1.a = _OutlineGradationColor.a;
outlineRainbowMixedColor2.a = _OutlineGradationColor.a;

// Select between normal gradation and rainbow gradation for outline
float4 outlineNormalGradation = select(_OutlineGradationLayer, _OutlineColor, outlineMixedColor0, outlineMixedColor1, outlineMixedColor2);
float4 outlineRainbowGradation = select(_OutlineGradationLayer, _OutlineColor, outlineRainbowMixedColor0, outlineRainbowMixedColor1, outlineRainbowMixedColor2);
float4 outlineColor = lerp(outlineNormalGradation, outlineRainbowGradation, _OutlineUseRainbow);
#else
float4 outlineColor = _OutlineColor;
#endif

/**
*
*
*
*/

#if 1
float graphicGradationAngle = PI * _GraphicGradationAngle;
#ifdef SDF_UI_TEX
float graphicGradationSmooth = _GraphicGradationSmooth;
float graphicGradationRadius = _GraphicGradationRadius;
float2 graphicGradationOffset = _GraphicGradationOffset.xy + float2(.5, .5);
#else
float graphicGradationSmooth = _GraphicGradationSmooth * minSize;
float graphicGradationRadius = _GraphicGradationRadius * minSize;
float2 graphicGradationOffset = _GraphicGradationOffset.xy * _RectSize.xy;
#endif
float2 graphicGradationPosition = rotate(pRotated - graphicGradationOffset, graphicGradationAngle);
float4 graphicMixedColor0 = linearGradation(graphicGradationPosition, graphicGradationSmooth, color, _GraphicGradationColor);
float4 graphicMixedColor1 = radialGradation(graphicGradationPosition, graphicGradationRadius, graphicGradationSmooth, color, _GraphicGradationColor);
float4 graphicMixedColor2 = conicalGradation(graphicGradationPosition, graphicGradationSmooth, _GraphicGradationRange, color, _GraphicGradationColor);

// Rainbow gradient implementation
float4 rainbowMixedColor0 = rainbowLinearGradation(graphicGradationPosition, graphicGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 rainbowMixedColor1 = rainbowRadialGradation(graphicGradationPosition, graphicGradationRadius, graphicGradationSmooth, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);
float4 rainbowMixedColor2 = rainbowConicalGradation(graphicGradationPosition, graphicGradationSmooth, _GraphicGradationRange, _GraphicRainbowSaturation, _GraphicRainbowValue, _GraphicRainbowHueOffset);

// Apply alpha from gradation color to rainbow colors
rainbowMixedColor0.a = _GraphicGradationColor.a;
rainbowMixedColor1.a = _GraphicGradationColor.a;
rainbowMixedColor2.a = _GraphicGradationColor.a;

// Select between normal gradation and rainbow gradation
float4 normalGradation = select(_GraphicGradationLayer, color, graphicMixedColor0, graphicMixedColor1, graphicMixedColor2);
float4 rainbowGradation = select(_GraphicGradationLayer, color, rainbowMixedColor0, rainbowMixedColor1, rainbowMixedColor2);
float4 graphicColor = lerp(normalGradation, rainbowGradation, _GraphicUseRainbow);
#else
float4 graphicColor = color;
#endif

/**
*
*
*
*/

{
#if defined(SDF_UI_OUTLINE_EFFECT_SHINY) || defined(SDF_UI_OUTLINE_EFFECT_PATTERN)
#if SDF_UI_OUTLINE_EFFECT_PATTERN
	_OutlineEffectColor.a /= outlinePatternAlpha;
	_OutlineEffectColor.a *= 1. - saturaterange(_OutlineEffectPatternParams.z, _OutlineEffectPatternParams.z + _OutlineEffectPatternParams.w, outlinePatternFade0);
#endif
	half4 layer0 = half4(lerp(outlineColor.rgb, _OutlineEffectColor.rgb, outlineEffect * _OutlineEffectColor.a), outlineColor.a); layer0.rgb *= layer0.a;
#else
	half4 layer0 = outlineColor; layer0.rgb *= layer0.a;
#endif

#if defined(SDF_UI_GRAPHIC_EFFECT_SHINY) || defined(SDF_UI_GRAPHIC_EFFECT_PATTERN)
#if SDF_UI_GRAPHIC_EFFECT_PATTERN
	_GraphicEffectColor.a /= graphicPatternAlpha;
	_GraphicEffectColor.a *= 1. - saturaterange(_GraphicEffectPatternParams.z, _GraphicEffectPatternParams.z + _GraphicEffectPatternParams.w, graphicPatternFade0);
#endif
	half4 layer1 = half4(lerp(color.rgb, _GraphicEffectColor.rgb, graphicEffect * _GraphicEffectColor.a), graphicColor.a); layer1.rgb *= layer1.a;
#else
	half4 layer1 = graphicColor; layer1.rgb *= layer1.a;
#endif

	half4 layer2 = lerp(layer0, layer1, graphicAlpha);
	effects = layer2 * outlineAlpha;
}

#endif	// SDF_UI_STEP_SHAPE_AND_OUTLINE

//////////////////////////////////////////////////////////////