/**
* SDF fragment after determined distance
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#ifdef SDF_UI_AA_SUBPIXEL
float4 delta = half4(0, 0, 0, 0);
#else
float delta = 0;
#endif

#endif // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHADOW
#ifdef SDF_UI_SHADOW_ENABLED

#ifdef SDF_UI_AA_SUBPIXEL
delta = fwidth(dist) * .5;
#elif defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

#ifdef SDF_UI_AA_SUBPIXEL
float4 alpha = 0;
#else
float alpha = 0;
#endif

#ifdef SDF_UI_OUTLINE_INSIDE
alpha = 1 - smoothstep((_ShadowWidth - _ShadowDilate) - _ShadowBlur - delta, (_ShadowWidth - _ShadowDilate) + delta, dist);
#elif SDF_UI_OUTLINE_OUTSIDE
alpha = 1 - smoothstep(_OutlineWidth + (_ShadowWidth - _ShadowDilate) - _ShadowBlur - delta, _OutlineWidth + (_ShadowWidth - _ShadowDilate) + delta, dist);
#endif

{
	half4 layer0 = _ShadowColor;
	layer0.a *= alpha;
	layer0.a *= (1. - effects.a);
	layer0.rgb *= layer0.a;
	effects = effects + layer0;
}

#endif	// SDF_UI_SHADOW_ENABLED
#endif	// SDF_UI_STEP_SHADOW

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHAPE_OUTLINE

#ifdef SDF_UI_AA_SUBPIXEL
delta = fwidth(dist) * .5;
#elif defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)
delta = fwidth(dist) * .5;
#else
delta = 0;
#endif

#ifdef SDF_UI_AA_SUBPIXEL
float4 graphicAlpha = 0, outlineAlpha = 0;
#else
float graphicAlpha = 0, outlineAlpha = 0;
#endif

#if defined(SDF_UI_AA_SUBPIXEL) || defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)

#ifdef SDF_UI_OUTLINE_INSIDE
graphicAlpha = 1 - smoothstep(-_OutlineWidth - delta, -_OutlineWidth + delta, dist);
outlineAlpha = 1 - smoothstep(-delta, delta, dist);
#elif SDF_UI_OUTLINE_OUTSIDE
outlineAlpha = 1 - smoothstep(_OutlineWidth - delta, _OutlineWidth + delta, dist);
graphicAlpha = 1 - smoothstep(-delta, delta, dist);
#endif

#else
graphicAlpha = 1 - (dist >= -_OutlineWidth);
outlineAlpha = 1 - (dist >= 0);
#endif

{
	half4 layer0 = _OutlineColor; layer0.rgb *= layer0.a;
	half4 layer1 = color; layer1.rgb *= layer1.a;
	half4 layer2 = lerp(layer0, layer1, graphicAlpha);
	effects = layer2 * outlineAlpha;
}

#endif	// SDF_UI_STEP_SHAPE_OUTLINE

//////////////////////////////////////////////////////////////