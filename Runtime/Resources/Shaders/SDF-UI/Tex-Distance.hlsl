/**
* SDF fragment to determin distance from shape (Tex.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = -(tex2D(_SDFTex, p)).a;
dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Radius);

#endif

//////////////////////////////////////////////////////////////