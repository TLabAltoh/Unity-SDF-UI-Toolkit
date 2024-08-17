/**
* SDF fragment to determin distance from shape (Tex.shader)
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
    -(tex2D(_SDFTex, p + mul(j, float2(1, 1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(1, -1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(-1, 1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(-1, -1) * 0.25))).a);

dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#elif SDF_UI_AA_SUBPIXEL
j = JACOBIAN(p);
r = -(tex2D(_SDFTex, p + mul(j, float2(-0.333, 0)))).a;
g = -(tex2D(_SDFTex, p)).a;
b = -(tex2D(_SDFTex, p + mul(j, float2(0.333, 0)))).a;
dist = half4(r, g, b, (r + g + b) / 3.);

dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#else
dist = -(tex2D(_SDFTex, p)).a;
dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Radius);

#endif

//////////////////////////////////////////////////////////////