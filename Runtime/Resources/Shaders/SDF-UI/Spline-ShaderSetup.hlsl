/***
* This code is adapted from
* https://www.shadertoy.com/view/dls3Wr
**/

#pragma multi_compile_local _ SDF_UI_SPLINE_FILL

#include "Spline-Properties.hlsl"

#ifdef SDF_UI_AA_SUBPIXEL
inline half4
#else
inline half
#endif
sdBezierAA(float2 p, float2x2 j, float2 A, float2 B, float2 C) {
#ifdef SDF_UI_AA_SUPER_SAMPLING
	// Super sampling is disabled because there is a possibility that the distance became zero on the edge of the sign (-1, +1).
	// ex: +2 + 2 - 2 - 2 == 0
	return sdBezier(p, A, B, C);
#elif SDF_UI_AA_SUBPIXEL
	j = JACOBIAN(p);
	float r = sdBezier(p + mul(j, float2(-0.333, 0)), A, B, C);
	float g = sdBezier(p, A, B, C);
	float b = sdBezier(p + mul(j, float2(0.333, 0)), A, B, C);
	return half4(r, g, b, (r + g + b) / 3.);
#else
	return sdBezier(p, A, B, C);
#endif
}

#ifdef SDF_UI_AA_SUBPIXEL
inline half4
#else
inline half
#endif
udSegmentAA(float2 p, float2x2 j, float2 A, float2 B) {
#ifdef SDF_UI_AA_SUPER_SAMPLING
	return udSegment(p, A, B);
#elif SDF_UI_AA_SUBPIXEL
	float r = udSegment(p + mul(j, float2(-0.333, 0)), A, B);
	float g = udSegment(p, A, B);
	float b = udSegment(p + mul(j, float2(0.333, 0)), A, B);
	return half4(r, g, b, (r + g + b) / 3.);
#else
	return udSegment(p, A, B);
#endif
}

#ifdef SDF_UI_AA_SUBPIXEL
inline half4
#else
inline half
#endif
distanceAA(float2 p, float2x2 j, float2 A) {
#ifdef SDF_UI_AA_SUPER_SAMPLING
	return distance(p, A);
#elif SDF_UI_AA_SUBPIXEL
	float r = distance(p + mul(j, float2(-0.333, 0)), A);
	float g = distance(p, A);
	float b = distance(p + mul(j, float2(0.333, 0)), A);
	return half4(r, g, b, (r + g + b) / 3.);
#else
	return distance(p, A);
#endif
}