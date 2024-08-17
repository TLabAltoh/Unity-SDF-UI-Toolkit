/**
* SDF fragment to determin distance from shape (Spline.shader)
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
    sdTriangle(p + mul(j, float2(1, 1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(-1, 1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy) +
    sdTriangle(p + mul(j, float2(-1, -1) * 0.25), _Corner0.xy, _Corner1.xy, _Corner2.xy));
#elif SDF_UI_AA_SUBPIXEL
j = JACOBIAN(p);
r = sdTriangle(p + mul(j, float2(-0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
g = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
b = sdTriangle(p + mul(j, float2(0.333, 0)), _Corner0.xy, _Corner1.xy, _Corner2.xy);
dist = half4(r, g, b, (r + g + b) / 3.);
#else
dist = sdTriangle(p, _Corner0.xy, _Corner1.xy, _Corner2.xy);
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Radius);

#endif

//////////////////////////////////////////////////////////////