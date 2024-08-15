/**
* SDF fragment to determin distance from shape (Triangle.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float2x2 j = JACOBIAN(p);
float dist = 0.25 * (
    sdTriangle(p + mul(j, float2(1, 1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(-1, 1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(-1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy));
#elif SDF_UI_AA_SUBPIXEL
float2x2 j = JACOBIAN(p);
float r = sdTriangle(p + mul(j, float2(-0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
float g = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
float b = sdTriangle(p + mul(j, float2(0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
float4 dist = half4(r, g, b, (r + g + b) / 3.);
#else
float dist = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Radius);