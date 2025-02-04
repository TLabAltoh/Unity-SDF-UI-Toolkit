/**
* SDF fragment to determin sampling position
*/

#ifdef SDF_UI_STEP_SETUP
float2 p;
#endif

#ifdef SDF_UI_TEX

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE
p = uv;
#endif

#ifdef SDF_UI_STEP_SHADOW
p = uv - _ShadowOffset.xy;
#endif

#else

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE
p = (uv - .5) * (halfSize + _OnionWidth) * 2;
#endif

#ifdef SDF_UI_STEP_SHADOW
p = (uv - .5 - _ShadowOffset.xy) * (halfSize + _OnionWidth) * 2;
#endif

#endif