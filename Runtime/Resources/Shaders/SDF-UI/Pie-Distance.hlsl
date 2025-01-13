/**
* SDF fragment to determin distance from shape (Pie.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist;
float2 sincos;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

if (_Theta >= 3.14) {
    dist = length(p) - _Radius;
}
else {
    sincos = float2(sin(_Theta), cos(_Theta));
    dist = sdPie(p, sincos, _Radius);
}

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Roundness);

#endif

//////////////////////////////////////////////////////////////