/**
* SDF fragment to determin distance from shape (Ring.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#ifdef SDF_UI_AA_SUBPIXEL
float4 dist;
float r, g, b;
#else
float dist;
#endif

float radius;
float2 cossin, t;
float2x2 j;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

t = p;
p.x = t.x * _AngleOffset.x - t.y * _AngleOffset.y;
p.y = t.x * _AngleOffset.y + t.y * _AngleOffset.x;

#ifdef SDF_UI_AA_SUPER_SAMPLING
if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    dist = 0.25 * (
        abs(length(p + mul(j, float2(1, 1) * 0.25)) - _Radius) - _CircleBorder +
        abs(length(p + mul(j, float2(1, -1) * 0.25)) - _Radius) - _CircleBorder +
        abs(length(p + mul(j, float2(-1, 1) * 0.25)) - _Radius) - _CircleBorder +
        abs(length(p + mul(j, float2(-1, -1) * 0.25)) - _Radius) - _CircleBorder);
}
else {
    j = JACOBIAN(p);
    cossin = float2(cos(_Theta), sin(_Theta));
    dist = 0.25 * (
        sdRing(p + mul(j, float2(1, 1) * 0.25), cossin, _Radius, _Width, _CornersRounding) +
        sdRing(p + mul(j, float2(1, -1) * 0.25), cossin, _Radius, _Width, _CornersRounding) +
        sdRing(p + mul(j, float2(-1, 1) * 0.25), cossin, _Radius, _Width, _CornersRounding) +
        sdRing(p + mul(j, float2(-1, -1) * 0.25), cossin, _Radius, _Width, _CornersRounding));
}
#elif SDF_UI_AA_SUBPIXEL
if (_Theta >= 3.14) {
    j = JACOBIAN(p);
    r = abs(length(p + mul(j, float2(-0.333, 0))) - _Radius) - _CircleBorder;
    g = abs(length(p) - _Radius) - _CircleBorder;
    b = abs(length(p + mul(j, float2(0.333, 0))) - _Radius) - _CircleBorder;
    dist = half4(r, g, b, (r + g + b) / 3.);
}
else {
    j = JACOBIAN(p);
    cossin = float2(cos(_Theta), sin(_Theta));
    r = sdRing(p + mul(j, float2(-0.333, 0)), cossin, _Radius, _Width, _CornersRounding);
    g = sdRing(p, cossin, _Radius, _Width, _CornersRounding);
    b = sdRing(p + mul(j, float2(0.333, 0)), cossin, _Radius, _Width, _CornersRounding);
    dist = half4(r, g, b, (r + g + b) / 3.);
}
#else
if (_Theta >= 3.14) {
    dist = abs(length(p) - _Radius) - _CircleBorder;
}
else {
    cossin = float2(cos(_Theta), sin(_Theta));
    dist = sdRing(p, cossin, _Radius, _Width, _CornersRounding);
}
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////