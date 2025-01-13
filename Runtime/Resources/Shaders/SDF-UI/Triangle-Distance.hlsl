/**
* SDF fragment to determin distance from shape (Triangle.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

dist = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Roundness);

#endif

//////////////////////////////////////////////////////////////