/**
* SDF fragment to determin distance from shape (Arc.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#ifdef SDF_UI_AA_SUBPIXEL
float4 dist;
float r, g, b;
#else
float dist;
#endif

float2x2 j;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

#ifdef SDF_UI_AA_SUPER_SAMPLING
if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    dist = 0.25 * (
        abs(length(p + mul(j, float2(1, 1) * 0.25)) - _Radius) - _Width +
        abs(length(p + mul(j, float2(1, -1) * 0.25)) - _Radius) - _Width +
        abs(length(p + mul(j, float2(-1, 1) * 0.25)) - _Radius) - _Width +
        abs(length(p + mul(j, float2(-1, -1) * 0.25)) - _Radius) - _Width);
}
else {
    j = JACOBIAN(p);
    dist = 0.25 * (
        sdArc(p + mul(j, float2(1, 1) * 0.25), float2(sin(_Theta), cos(_Theta)), _Radius, _Width) +
        sdArc(p + mul(j, float2(1, -1) * 0.25), float2(sin(_Theta), cos(_Theta)), _Radius, _Width) +
        sdArc(p + mul(j, float2(-1, 1) * 0.25), float2(sin(_Theta), cos(_Theta)), _Radius, _Width) +
        sdArc(p + mul(j, float2(-1, -1) * 0.25), float2(sin(_Theta), cos(_Theta)), _Radius, _Width));
}
#elif SDF_UI_AA_SUBPIXEL
if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    r = abs(length(p + mul(j, float2(-0.333, 0))) - _Radius) - _Width;
    g = abs(length(p) - _Radius) - _Width;
    b = abs(length(p + mul(j, float2(0.333, 0))) - _Radius) - _Width;
    dist = half4(r, g, b, (r + g + b) / 3.);
}
else {
    j = JACOBIAN(p);
    r = sdArc(p + mul(j, float2(-0.333, 0)), float2(sin(_Theta), cos(_Theta)), _Radius, _Width);
    g = sdArc(p, float2(sin(_Theta), cos(_Theta)), _Radius, _Width);
    b = sdArc(p + mul(j, float2(0.333, 0)), float2(sin(_Theta), cos(_Theta)), _Radius, _Width);
    dist = half4(r, g, b, (r + g + b) / 3.);
}
#else
if (_Theta >= 3.14) {
    dist = abs(length(p) - _Radius) - _Width;
}
else {
    dist = sdArc(p, float2(sin(_Theta), cos(_Theta)), _Radius, _Width);
}
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////