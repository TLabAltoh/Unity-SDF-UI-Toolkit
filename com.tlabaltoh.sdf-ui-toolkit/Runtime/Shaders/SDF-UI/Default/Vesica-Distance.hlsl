/**
* 
* SDF fragment to determin distance from shape (Vesica.shader)
* 
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdVesica(p, halfSize.x - _Roundness, halfSize.y - _Roundness);

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

dist = round(dist, _Roundness);

#endif

//////////////////////////////////////////////////////////////