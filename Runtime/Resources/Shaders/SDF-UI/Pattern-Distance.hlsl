/**
* SDF fragment to determin distance from shape (Quad.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#if defined(SDF_UI_OUTLINE_PATTERN_SHINY) || defined(SDF_UI_OUTLINE_PATTERN_TEX)
float outlinePattern;
#endif

#if defined(SDF_UI_GRAPHIC_PATTERN_SHINY) || defined(SDF_UI_GRAPHIC_PATTERN_TEX)
float graphicPattern;
#endif

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE

#ifdef SDF_UI_OUTLINE_PATTERN_SHINY
outlinePattern = shiny(p - _OutlinePatternOffset, _OutlineShinyWidth, _OutlineShinyAngle, _OutlineShinyBlur);
#elif SDF_UI_OUTLINE_PATTERN_TEX
outlinePattern = 0;
#endif

#ifdef SDF_UI_GRAPHIC_PATTERN_SHINY
graphicPattern = shiny(p - _GraphicPatternOffset, _GraphicShinyWidth, _GraphicShinyAngle, _GraphicShinyBlur);
#elif SDF_UI_GRAPHIC_PATTERN_TEX
graphicPattern = 0;
#endif

#endif

//////////////////////////////////////////////////////////////