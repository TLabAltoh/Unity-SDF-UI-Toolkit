/**
* SDF fragment to determin this pixel is in/out of shape (Spline.shader)
*/

// Size of pixel in glyph space
float2 pos = -_RectSize.xy / 2 + _RectSize.xy * i.uv;
float pixelSize = ddx(pos.x);
float alphaSum = 0;

// Render 3 times (with slight y offset) for anti-aliasing
for (int yOffset = -1; yOffset <= 1; yOffset++)
{
	float2 samplePos = p + float2(0, yOffset) * pixelSize / 3.0;
	float coverage = calculateHorizontalCoverage(samplePos, pixelSize);
	alphaSum += coverage;
}

float alpha = alphaSum / 3.0;
effects = _Color * alpha;