/**
* SDF fragment to determin distance from shape (Tex.shader)
*/

#ifdef SDF_UI_AA_SUPER_SAMPLING
float2x2 j = JACOBIAN(p);
float dist = 0.25 * (
    -(tex2D(_SDFTex, p + mul(j, float2(1, 1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(1, -1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(-1, 1) * 0.25))).a
    - (tex2D(_SDFTex, p + mul(j, float2(-1, -1) * 0.25))).a);

dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#elif SDF_UI_AA_SUBPIXEL
float2x2 j = JACOBIAN(p);
float r = -(tex2D(_SDFTex, p + mul(j, float2(-0.333, 0)))).a;
float g = -(tex2D(_SDFTex, p)).a;
float b = -(tex2D(_SDFTex, p + mul(j, float2(0.333, 0)))).a;
float4 dist = half4(r, g, b, (r + g + b) / 3.);

dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#else
float dist = -(tex2D(_SDFTex, p)).a;
dist = dist * 2.0 + 1.0;
dist = dist * _MaxDist;
#endif

#ifdef SDF_UI_ONION
dist = abs(dist) - _OnionWidth;
#endif

dist = round(dist, _Radius);