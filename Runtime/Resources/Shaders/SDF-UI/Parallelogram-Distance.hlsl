/**
* 
* SDF fragment to determin distance from shape (Parallelogram.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdParallelogram(p, halfSize.x - 10, halfSize.y - 10, _Slide);

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

#endif

//////////////////////////////////////////////////////////////