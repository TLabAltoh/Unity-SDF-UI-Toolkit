/**
* SDF fragment after determined distance
*/

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

float shadowAlpha = softAlpha0 * (1. - _ShadowGaussian) + softAlpha1 * _ShadowGaussian;

{
	half4 layer0 = _ShadowColor;
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

graphicAlpha = softAlpha0 * (1. - _OutlineInnerGaussian) + softAlpha1 * _OutlineInnerGaussian;

{
#if defined(SDF_UI_OUTLINE_PATTERN_SHINY) || defined(SDF_UI_OUTLINE_PATTERN_TEX)
	half4 layer0 = half4(lerp(_OutlineColor.rgb, _OutlinePatternColor.rgb, outlinePattern * _OutlinePatternColor.a), _OutlineColor.a); layer0.rgb *= layer0.a;
#else
	half4 layer0 = _OutlineColor; layer0.rgb *= layer0.a;
#endif

#if defined(SDF_UI_GRAPHIC_PATTERN_SHINY) || defined(SDF_UI_GRAPHIC_PATTERN_TEX)
	half4 layer1 = half4(lerp(color.rgb, _GraphicPatternColor.rgb, graphicPattern * _GraphicPatternColor.a), color.a); layer1.rgb *= layer1.a;
#else
	half4 layer1 = color; layer1.rgb *= layer1.a;
#endif

	half4 layer2 = lerp(layer0, layer1, graphicAlpha);
	effects = layer2 * outlineAlpha;
}

#endif	// SDF_UI_STEP_SHAPE_AND_OUTLINE

//////////////////////////////////////////////////////////////