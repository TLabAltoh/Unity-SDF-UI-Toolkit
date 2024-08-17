/**
* SDF fragment to determin distance from shape (CutDisk.shader)
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
    sdCutDisk(p + mul(j, float2(1, 1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(1, -1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(-1, 1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(-1, -1) * 0.25), _Radius, _Height));
#elif SDF_UI_AA_SUBPIXEL
j = JACOBIAN(p);
r = sdCutDisk(p + mul(j, float2(-0.333, 0)), _Radius, _Height);
g = sdCutDisk(p, _Radius, _Height);
b = sdCutDisk(p + mul(j, float2(0.333, 0)), _Radius, _Height);
dist = half4(r, g, b, (r + g + b) / 3.);
#else
dist = sdCutDisk(p, _Radius, _Height);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

#endif

//////////////////////////////////////////////////////////////