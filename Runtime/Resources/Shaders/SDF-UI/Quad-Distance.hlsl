/**
* SDF fragment to determin distance from shape (Quad.shader)
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
j = JACOBIAN(p);
dist = 0.25 * (
    sdRoundedBox(p + mul(j, float2(1, 1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(1, -1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(-1, 1) * 0.25), halfSize, _Radius) +
    sdRoundedBox(p + mul(j, float2(-1, -1) * 0.25), halfSize, _Radius));
#elif SDF_UI_AA_SUBPIXEL
j = JACOBIAN(p);
r = sdRoundedBox(p + mul(j, float2(-0.333, 0)), halfSize, _Radius);
g = sdRoundedBox(p, halfSize, _Radius);
b = sdRoundedBox(p + mul(j, float2(0.333, 0)), halfSize, _Radius);
dist = half4(r, g, b, (r + g + b) / 3.);
#else
dist = sdRoundedBox(p, halfSize, _Radius);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////