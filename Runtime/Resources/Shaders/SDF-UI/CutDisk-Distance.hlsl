/**
* SDF fragment to determin distance from shape (CutDisk.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdCutDisk(p, _Radius, _Height);

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////