/**
* SDF fragment to determin distance from shape (Quad.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float2x2 j = JACOBIAN(p);
float dist = 0.25 * (
    sdRoundedBox(p + mul(j, float2(1, 1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(1, -1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(-1, 1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(-1, -1) * 0.25), halfSize, _Radius));
#elif SDF_UI_AA_SUBPIXEL
float2x2 j = JACOBIAN(p);
float r = sdRoundedBox(p + mul(j, float2(-0.333, 0)), halfSize, _Radius);
float g = sdRoundedBox(p, halfSize, _Radius);
float b = sdRoundedBox(p + mul(j, float2(0.333, 0)), halfSize, _Radius);
float4 dist = half4(r, g, b, (r + g + b) / 3.);
#else
float dist = sdRoundedBox(p, halfSize, _Radius);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif