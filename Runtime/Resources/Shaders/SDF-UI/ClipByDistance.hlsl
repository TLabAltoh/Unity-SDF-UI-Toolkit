/**
* SDF fragment after determined distance
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float delta = 0, softness = 0, tmp0 = 0, tmp1 = 0;

#endif // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHADOW
#ifdef SDF_UI_SHADOW_ENABLED

#ifdef SDF_UI_AA
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

tmp0 = 1 - saturaterange(_ShadowBorder - _ShadowBlur - delta, _ShadowBorder + delta, dist);
tmp1 = 1 - smoothstep(_ShadowBorder - _ShadowBlur - delta, _ShadowBorder + delta, dist);
softness = tmp0 * (1. - _ShadowGaussian) + tmp1 * _ShadowGaussian;

{
	half4 layer0 = _ShadowColor;
	layer0.a *= softness;
	layer0.a *= (1. - effects.a);
	layer0.rgb *= layer0.a;
	effects = effects + layer0;
}

#endif	// SDF_UI_SHADOW_ENABLED
#endif	// SDF_UI_STEP_SHADOW

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHAPE_OUTLINE

#ifdef SDF_UI_AA
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

float graphicAlpha = 0, outlineAlpha = 0;

tmp0 = saturaterange(_GraphicBorder - _OutlineInnerBlur, _GraphicBorder, dist);
tmp1 = smoothstep(_GraphicBorder - _OutlineInnerBlur, _GraphicBorder, dist);
softness = tmp0 * (1. - _OutlineInnerGaussian) + tmp1 * _OutlineInnerGaussian;

#ifdef SDF_UI_AA
outlineAlpha = 1 - saturaterange(_OutlineBorder - delta, _OutlineBorder + delta, dist);
graphicAlpha = 1 - saturaterange(_GraphicBorder - delta, _GraphicBorder + delta, dist);
#else
graphicAlpha = 1 - (dist >= _GraphicBorder);
outlineAlpha = 1 - (dist >= _OutlineBorder);
#endif

{
	half4 layer0 = _OutlineColor; layer0.rgb *= layer0.a;
	half4 layer1 = color; layer1.rgb *= layer1.a;
	half4 layer2 = lerp(layer0, layer1, graphicAlpha);
	effects = lerp(layer2, layer0, softness) * outlineAlpha;
}

#endif	// SDF_UI_STEP_SHAPE_OUTLINE

//////////////////////////////////////////////////////////////