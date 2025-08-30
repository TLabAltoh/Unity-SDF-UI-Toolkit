/**
* 
* SDF fragment to determin distance from shape (Pie.shader)
* 
*/

#define PI 3.14

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

float dist, onion;
float2 sincos;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_AND_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

if (_Theta >= PI) {
    dist = length(p) - _Radius;
}
else {
    sincos = float2(sin(_Theta), cos(_Theta));
    dist = sdPie(p, sincos, _Radius);
}

onion = abs(dist) - _OnionWidth;

dist = dist * (1. - _Onion) + onion * _Onion;

dist = round(dist, _Roundness);

#endif

//////////////////////////////////////////////////////////////