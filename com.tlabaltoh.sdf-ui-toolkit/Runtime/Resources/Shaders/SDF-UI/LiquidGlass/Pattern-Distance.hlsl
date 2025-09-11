/**
* SDF fragment to determin distance from shape (Quad.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#if defined(SDF_UI_OUTLINE_EFFECT_SHINY) || defined(SDF_UI_OUTLINE_EFFECT_PATTERN)
float outlineEffect;
#endif

#if defined(SDF_UI_GRAPHIC_EFFECT_SHINY) || defined(SDF_UI_GRAPHIC_EFFECT_PATTERN)
float graphicEffect;
#endif

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE

#ifdef SDF_UI_OUTLINE_EFFECT_SHINY
outlineEffect = shiny(p - _OutlineEffectOffset * halfSize, _OutlineEffectShinyWidth, _OutlineEffectAngle + _EulerZ, _OutlineEffectShinyBlur);
#elif SDF_UI_OUTLINE_EFFECT_PATTERN

outlineEffect = -(tex2D(_OutlineEffectPatternTex, float2(outlinePatternSample.x, 1.0 - outlinePatternSample.y)).a;
outlineEffect = outlineEffect * 2.0 + 1.0;

#ifdef SDF_UI_AA
delta = fwidth(outlineEffect) * .5;
outlineEffect = 1 - saturaterange(-delta, delta, outlineEffect);
#else
outlineEffect = 1 - (outlineEffect >= 0);
#endif

#endif

#ifdef SDF_UI_GRAPHIC_EFFECT_SHINY
graphicEffect = shiny(p - _GraphicEffectOffset * halfSize, _GraphicEffectShinyWidth, _GraphicEffectAngle + _EulerZ, _GraphicEffectShinyBlur);
#elif SDF_UI_GRAPHIC_EFFECT_PATTERN

graphicEffect = -(tex2D(_GraphicEffectPatternTex, float2(graphicPatternSample.x, 1.0 - graphicPatternSample.y))).a;
graphicEffect = graphicEffect * 2.0 + 1.0;

#ifdef SDF_UI_AA
delta = fwidth(graphicEffect) * .5;
graphicEffect = 1 - saturaterange(-delta, delta, graphicEffect);
#else
graphicEffect = 1 - (graphicEffect >= 0);
#endif

#endif
#endif

//////////////////////////////////////////////////////////////