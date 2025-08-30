/**
* 
* SDF fragment to determin distance from shape (Triangle.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

dist = round(dist, _Roundness);

#endif

//////////////////////////////////////////////////////////////