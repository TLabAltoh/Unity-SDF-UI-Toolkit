/**
* SDF fragment to determin distance from shape (Circle-Outline.shader)
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
    length(p + mul(j, float2(1, 1) * 0.25)) - _Radius +
    length(p + mul(j, float2(1, -1) * 0.25)) - _Radius +
    length(p + mul(j, float2(-1, 1) * 0.25)) - _Radius +
    length(p + mul(j, float2(-1, -1) * 0.25)) - _Radius);
#elif SDF_UI_AA_SUBPIXEL
j = JACOBIAN(p);
r = length(p + mul(j, float2(-0.333, 0))) - _Radius;
g = length(p) - _Radius;
b = length(p + mul(j, float2(0.333, 0))) - _Radius;
dist = half4(r, g, b, (r + g + b) / 3.);
#else
dist = length(p) - _Radius;
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////