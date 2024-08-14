/**
* SDF fragment to determin distance from shape (Ring.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float4 dist;
float2x2 j;

if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    dist = 0.25 * (
        abs(length(p + mul(j, float2(1, 1) * 0.25)) - _Radius) - _Width * .5 +
        abs(length(p + mul(j, float2(1, -1) * 0.25)) - _Radius) - _Width * .5 +
        abs(length(p + mul(j, float2(-1, 1) * 0.25)) - _Radius) - _Width * .5 +
        abs(length(p + mul(j, float2(-1, -1) * 0.25)) - _Radius) - _Width * .5);
}
else {
    j = JACOBIAN(p);
    dist = 0.25 * (
        sdRing(p + mul(j, float2(1, 1) * 0.25), float2(cos(_Theta), sin(_Theta)), _Radius, _Width) +
        sdRing(p + mul(j, float2(1, -1) * 0.25), float2(cos(_Theta), sin(_Theta)), _Radius, _Width) +
        sdRing(p + mul(j, float2(-1, 1) * 0.25), float2(cos(_Theta), sin(_Theta)), _Radius, _Width) +
        sdRing(p + mul(j, float2(-1, -1) * 0.25), float2(cos(_Theta), sin(_Theta)), _Radius, _Width));
}
#elif SDF_UI_AA_SUBPIXEL
float4 dist, sdist;
float2x2 j;
float r, g, b;

if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    r = abs(length(p + mul(j, float2(-0.333, 0))) - _Radius) - _Width * .5;
    g = abs(length(p) - _Radius) - _Width * .5;
    b = abs(length(p + mul(j, float2(0.333, 0))) - _Radius) - _Width * .5;
    dist = half4(r, g, b, (r + g + b) / 3.);
}
else {
    j = JACOBIAN(p);
    r = sdRing(p + mul(j, float2(-0.333, 0)), float2(cos(_Theta), sin(_Theta)), _Radius, _Width);
    g = sdRing(p, float2(cos(_Theta), sin(_Theta)), _Radius, _Width);
    b = sdRing(p + mul(j, float2(0.333, 0)), float2(cos(_Theta), sin(_Theta)), _Radius, _Width);
    dist = half4(r, g, b, (r + g + b) / 3.);
}
#else
float dist, sdist;
if (_Theta >= 3.14) {
    dist = abs(length(p) - _Radius) - _Width * .5;
}
else {
    dist = sdRing(p, float2(cos(_Theta), sin(_Theta)), _Radius, _Width);
}
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif