/**
* SDF fragment to determin sampling position
*/

#ifdef SDF_UI_STEP_SETUP
float2 p, pRotated;
#endif

#ifdef SDF_UI_TEX

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE
p = uv;
pRotated = rotate(p, _EulerZ);
#endif

#ifdef SDF_UI_STEP_SHADOW
p = uv - _ShadowOffset.xy;
pRotated = rotate(p, _EulerZ);
#endif

#else

#ifdef SDF_UI_STEP_SHAPE_AND_OUTLINE
p = (uv - .5) * (halfSize + _OnionWidth) * 2;
pRotated = rotate(p, _EulerZ);
#endif

#ifdef SDF_UI_STEP_SHADOW
p = (uv - .5 - _ShadowOffset.xy) * (halfSize + _OnionWidth) * 2;
pRotated = rotate(p, _EulerZ);
#endif

#endif