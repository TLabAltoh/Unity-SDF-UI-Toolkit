/**
* SDF fragment to determin distance from shape (CutDisk.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float2x2 j = JACOBIAN(p);
float dist = 0.25 * (
    sdCutDisk(p + mul(j, float2(1, 1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(1, -1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(-1, 1) * 0.25), _Radius, _Height) +
    sdCutDisk(p + mul(j, float2(-1, -1) * 0.25), _Radius, _Height));
#elif SDF_UI_AA_SUBPIXEL
float2x2 j = JACOBIAN(p);
float r = sdCutDisk(p + mul(j, float2(-0.333, 0)), _Radius, _Height);
float g = sdCutDisk(p, _Radius, _Height);
float b = sdCutDisk(p + mul(j, float2(0.333, 0)), _Radius, _Height);
float4 dist = half4(r, g, b, (r + g + b) / 3.);
#else
float dist = sdCutDisk(p, _Radius, _Height);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif