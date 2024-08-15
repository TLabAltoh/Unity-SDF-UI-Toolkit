/**
* SDF fragment to determin distance from shape (Circle-Outline.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float4 dist;
float2x2 j;

j = JACOBIAN(p);
dist = 0.25 * (
    length(p + mul(j, float2(1, 1) * 0.25)) - _Radius +
    length(p + mul(j, float2(1, -1) * 0.25)) - _Radius +
    length(p + mul(j, float2(-1, 1) * 0.25)) - _Radius +
    length(p + mul(j, float2(-1, -1) * 0.25)) - _Radius);
#elif SDF_UI_AA_SUBPIXEL
float4 dist;
float2x2 j;
float r, g, b;

j = JACOBIAN(p);
r = length(p + mul(j, float2(-0.333, 0))) - _Radius;
g = length(p) - _Radius;
b = length(p + mul(j, float2(0.333, 0))) - _Radius;
dist = half4(r, g, b, (r + g + b) / 3.);
#else
float dist = length(p) - _Radius;
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif