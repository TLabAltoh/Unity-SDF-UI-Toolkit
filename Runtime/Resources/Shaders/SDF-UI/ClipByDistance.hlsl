/**
* SDF fragment after determined distance
*/

#if SDF_UI_STEP_SHADOW

#ifdef SDF_UI_AA_SUBPIXEL
float4 delta = fwidth(dist) * .5;
#elif defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)
float delta = fwidth(dist) * .5;
#else
float delta = 0;
#endif

#ifdef SDF_UI_AA_SUBPIXEL
float4 alpha = 0;
#else
float alpha = 0;
#endif

#ifdef SDF_UI_OUTLINE_INSIDE
alpha = 1 - smoothstep(_ShadowWidth - _ShadowBlur - delta, _ShadowWidth + delta, dist);
#elif SDF_UI_OUTLINE_OUTSIDE
alpha = 1 - smoothstep(_OutlineWidth + _ShadowWidth - _ShadowBlur - delta, _OutlineWidth + _ShadowWidth + delta, dist);
#endif

#ifdef SDF_UI_AA_SUBPIXEL
half4 layer0 = half4(_ShadowColor.rgb, _ShadowColor.a * alpha.a * pow(alpha.a, _ShadowPower));
#else
half4 layer0 = half4(_ShadowColor.rgb, _ShadowColor.a * alpha * pow(alpha, _ShadowPower));
#endif

half4 effects = layer0;

#else

#ifdef SDF_UI_AA_SUBPIXEL
float4 delta = fwidth(dist) * .5;
#elif defined(SDF_UI_AA_SUPER_SAMPLING) || defined(SDF_UI_AA_FASTER)
float delta = fwidth(dist) * .5;
#else
float delta = 0;
#endif

#ifdef SDF_UI_AA_SUBPIXEL
float4 graphicAlpha = 0, outlineAlpha = 0;
#else
float graphicAlpha = 0, outlineAlpha = 0;
#endif

#ifdef SDF_UI_OUTLINE_INSIDE
graphicAlpha = 1 - smoothstep(-_OutlineWidth - delta, -_OutlineWidth + delta, dist);
outlineAlpha = 1 - smoothstep(-delta, delta, dist);
#elif SDF_UI_OUTLINE_OUTSIDE
outlineAlpha = 1 - smoothstep(_OutlineWidth - delta, _OutlineWidth + delta, dist);
graphicAlpha = 1 - smoothstep(-delta, delta, dist);
#endif

#ifdef SDF_UI_AA_SUBPIXEL
half4 layer0 = lerp(half4(1, 1, 1, 0), _OutlineColor, outlineAlpha);
half4 layer1 = color * graphicAlpha;
#else
half4 layer0 = half4(_OutlineColor.rgb, _OutlineColor.a * outlineAlpha);
half4 layer1 = half4(color.rgb, color.a);
#endif
half4 layer2 = blend(layer0, layer1, _OutlineColor, color, graphicAlpha);

half4 effects = layer2;

#endif

effects *= i.color;

#ifdef UNITY_UI_CLIP_RECT
effects.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
#endif

#ifdef UNITY_UI_ALPHACLIP
clip(effects.a - 0.001 > 0.0 ? 1 : -1);
#endif

return effects;