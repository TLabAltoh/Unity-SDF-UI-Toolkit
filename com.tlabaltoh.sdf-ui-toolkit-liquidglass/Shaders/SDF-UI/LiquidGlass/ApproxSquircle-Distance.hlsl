/**
* 
* SDF fragment to determin distance from shape (ApproxSquircle.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion, hMinSize = _MinSize * .5;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdApproxSquircle(p / hMinSize, _Roundness) * hMinSize;

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

#endif

//////////////////////////////////////////////////////////////