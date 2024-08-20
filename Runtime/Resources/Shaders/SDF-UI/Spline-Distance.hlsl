/**
* SDF fragment to determin distance from shape (Spline.shader)
*/

//////////////////////////////////////////////////////////////

#ifdef SDF_UI_STEP_SETUP

#ifdef SDF_UI_AA_SUBPIXEL
float4 dist, tmp;
float r, g, b;
#else
float dist, tmp;
#endif

int idx = 0;
float2x2 j;

#endif  // SDF_UI_STEP_SETUP

//////////////////////////////////////////////////////////////

#if defined(SDF_UI_STEP_SHAPE_OUTLINE) || defined(SDF_UI_STEP_SHADOW)

#ifdef SDF_UI_AA_SUPER_SAMPLING
dist = half4(1, 1, 1, 1) * 3.402823466e+38F;
#else
dist = 3.402823466e+38F;
#endif

for (idx = 0; idx < (_Num - 2); idx += 2) {
    float2 v0 = _Controls[idx + 0];
    float2 v1 = _Controls[idx + 1];
    float2 v2 = _Controls[idx + 2];

#ifdef SDF_UI_AA_SUPER_SAMPLING
    j = JACOBIAN(p);
    tmp = 0.25 * (
        sdBezier(p + mul(j, float2(1, 1) * 0.25), v0, v1, v2) +
        sdBezier(p + mul(j, float2(1, -1) * 0.25), v0, v1, v2) +
        sdBezier(p + mul(j, float2(-1, 1) * 0.25), v0, v1, v2) +
        sdBezier(p + mul(j, float2(-1, -1) * 0.25), v0, v1, v2));
#elif SDF_UI_AA_SUBPIXEL
    j = JACOBIAN(p);
    r = sdBezier(p + mul(j, float2(-0.333, 0)), v0, v1, v2);
    g = sdBezier(p, v0, v1, v2);
    b = sdBezier(p + mul(j, float2(0.333, 0)), v0, v1, v2);
    tmp = half4(r, g, b, (r + g + b) / 3.);
#else
    tmp = sdBezier(p, v0, v1, v2);
#endif

    dist = min(dist, abs(tmp));
}

if (idx < _Num - 1) {
    float2 v0 = _Controls[idx + 0];
    float2 v1 = _Controls[idx + 1];

#ifdef SDF_UI_AA_SUPER_SAMPLING
    j = JACOBIAN(p);
    tmp = 0.25 * (
        udSegment(p + mul(j, float2(1, 1) * 0.25), v0, v1) +
        udSegment(p + mul(j, float2(1, -1) * 0.25), v0, v1) +
        udSegment(p + mul(j, float2(-1, 1) * 0.25), v0, v1) +
        udSegment(p + mul(j, float2(-1, -1) * 0.25), v0, v1));
#elif SDF_UI_AA_SUBPIXEL
    j = JACOBIAN(p);
    r = udSegment(p + mul(j, float2(-0.333, 0)), v0, v1);
    g = udSegment(p, v0, v1);
    b = udSegment(p + mul(j, float2(0.333, 0)), v0, v1);
    tmp = half4(r, g, b, (r + g + b) / 3.);
#else
    tmp = udSegment(p, v0, v1);
#endif

    dist = min(dist, tmp);
}

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Width);

#endif

//////////////////////////////////////////////////////////////