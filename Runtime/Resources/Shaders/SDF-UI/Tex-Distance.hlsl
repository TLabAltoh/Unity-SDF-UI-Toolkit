/**
* SDF fragment to determin distance from shape (Tex.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = -(tex2D(_SDFTex, p)).a;
dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

dist = round(dist, _Radius);

#endif

//////////////////////////////////////////////////////////////