/**
* SDF fragment to determin distance from shape (CutDisk.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdCutDisk(p, _Radius, _Height);

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

#endif

//////////////////////////////////////////////////////////////