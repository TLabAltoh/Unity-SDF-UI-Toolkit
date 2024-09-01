/***
* This code is adapted from
* https://github.com/SebLague/Text-Rendering/blob/main/Assets/Scripts/SebText/Renderer/Resources/TextShader.shader
* https://www.shadertoy.com/view/dls3Wr
**/

//#pragma multi_compile_local _ SDF_UI_SPLINE_FONT_RENDERING
#pragma multi_compile_local _ SDF_UI_SPLINE_FILL

#include "Spline-Properties.hlsl"

#ifdef SDF_UI_SPLINE_FONT_RENDERING
// Calculate roots of quadratic equation(value/s for which: aÅ~t ^ 2 + bÅ~t + c = 0)
float2 calculateQuadraticRoots(float a, float b, float c) {
	const float EPSILON = 1e-5;
	float2 roots = -99999;

	// For a straight line, solve: bÅ~t + c = 0; therefore t = -c/b
	if (abs(a) < EPSILON) {
		if (b != 0) roots[0] = -c / b;
	}
	else {
		// Solve using quadratic formula: t = (-b Å} sqrt(b^2 - 4ac)) / (2a)
		// If the value under the sqrt is negative, the equation has no real roots
		float discriminant = b * b - 4 * a * c;

		// Allow discriminant to be slightly negative to avoid a curve being missed due
		// to precision limitations. Must be clamped to zero before it's used in sqrt though!
		if (discriminant > -EPSILON) {
			float s = sqrt(max(0, discriminant));
			roots[0] = (-b + s) / (2 * a);
			roots[1] = (-b - s) / (2 * a);
		}
	}

	return roots;
}

// Calculate the fraction [0,1] of the pixel that is covered by the glyph (along the x axis).
// This is done by looking at the distances to the intersection points of a horizontal ray
// (at the pixel pos) with all the curves of the glyph.
float calculateHorizontalCoverage(float2 pixelPos, float pixelSize) {
	float coverage = 0;
	float invPixelSize = 1 / pixelSize;

	for (int i = 0; i < _SplinesNum - 2; i += 2) {
		// Get positions of curve's control points relative to the current pixel
		float2 p0 = _Splines[i + 0] - pixelPos;
		float2 p1 = _Splines[i + 1] - pixelPos;
		float2 p2 = _Splines[i + 2] - pixelPos;

		// Check if curve segment is going downwards (this means that a ray crossing
		// it from left to right would be exiting the shape at this point).
		// Note: curves are assumed to be monotonic (strictly increasing or decreasing on the y axis)
		bool isDownwardCurve = p0.y > 0 || p2.y < 0;

		// Skip curves that are entirely above or below the ray
		// When two curves are in the same direction (upward or downward), only one of them should be
		// counted at their meeting point to avoid double-counting. When in opposite directions, however,
		// the curve is not crossing the contour (but rather just grazing it) and so the curves should
		// either both be skipped, or both counted (so as not to affect the end result).
		if (isDownwardCurve) {
			if (p0.y < 0 && p2.y <= 0) continue;
			if (p0.y > 0 && p2.y >= 0) continue;
		}
		else {
			if (p0.y <= 0 && p2.y < 0) continue;
			if (p0.y >= 0 && p2.y > 0) continue;
		}

		// Calculate a,b,c of quadratic equation for current bezier curve
		float2 a = p0 - 2 * p1 + p2;
		float2 b = 2 * (p1 - p0);
		float2 c = p0;

		// Calculate roots to see if ray intersects curve segment.
		// Note: intersection is allowed slightly outside of [0, 1] segment to tolerate precision issues.
		const float EPSILON = 1e-4;
		float2 roots = calculateQuadraticRoots(a.y, b.y, c.y);
		bool onSeg0 = roots[0] >= -EPSILON && roots[0] <= 1 + EPSILON;
		bool onSeg1 = roots[1] >= -EPSILON && roots[1] <= 1 + EPSILON;

		// Calculate distance to intersection (negative if to left of ray)
		float t0 = saturate(roots[0]);
		float t1 = saturate(roots[1]);
		float intersect0 = a.x * t0 * t0 + b.x * t0 + c.x;
		float intersect1 = a.x * t1 * t1 + b.x * t1 + c.x;

		// Calculate the fraction of the ray that passes through the glyph (within the current pixel):
		// A value [0, 1] is calculated based on where the intersection occurs: 0 at the left edge of
		// the pixel, increasing to 1 at the right edge. This value is added to the total coverage
		// value when the ray exits a shape, and subtracted when the ray enters a shape.
		int sign = isDownwardCurve ? 1 : -1;
		if (onSeg0) coverage += saturate(0.5 + intersect0 * invPixelSize) * sign;
		if (onSeg1) coverage += saturate(0.5 + intersect1 * invPixelSize) * sign;
	}

	return saturate(coverage);
}
#else
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
#endif