using Unity.Burst;
using Unity.Mathematics;

namespace TLab.UI.SDF
{
	[BurstCompile]
	public static class MathUtils
	{
		[BurstCompile]
		public static void RotateVector(in float2 vector, in float rotation, out float2 rotated)
		{
			if (math.abs(rotation) < 0.0001f)
			{
				rotated = vector;
				return;
			}
			if (math.abs(vector.x) < 0.0001f && math.abs(vector.y) < 0.0001f)
			{
				rotated = vector;
				return;
			}

			math.sincos(math.radians(-rotation), out float sin, out float cos);
			float2x2 matrix = new(cos, -sin, sin, cos);
			rotated = math.mul(matrix, vector);
		}

		[BurstCompile]
		public static void Cross(in float2 a, in float2 b, out float c) => c = a.x * b.y - a.y * b.x;

		[BurstCompile]
		public static float Cross(in float2 a, in float2 b) => a.x * b.y - a.y * b.x;

		[BurstCompile]
		public static void QuadraticBezierDifferential(in float2 p0, in float2 p1, in float2 p2, in float t, out float2 diffrential)
		{
			diffrential = -2 * (1 - t) * p0 + 2 * (1 - 2 * t) * p1 + 2 * t * p2;
		}

		[BurstCompile]
		public static void QuadraticBezierTangentAndLeft(in float2 p0, in float2 p1, in float2 p2, in float t, out float2 tangent, out float2 left)
		{
			QuadraticBezierDifferential(p0, p1, p2, t, out var diffrential);
			tangent = math.normalize(diffrential);
			left = new float2(-tangent.y, tangent.x);
		}

		[BurstCompile]
		public static void QuadraticBezier(in float2 p0, in float2 p1, in float2 p2, in float t, out float2 point)
		{
			point = (1 - t) * (1 - t) * p0 + 2 * t * (1 - t) * p1 + t * t * p2;
		}

		[BurstCompile]
		public static void GetVectorDiff(in float3 p0, in float3 p1, out float3 diff, out float3 dir, out float len)
		{
			diff = p1 - p0;
			dir = math.normalize(diff);
			len = math.length(diff);
		}

		[BurstCompile]
		public static void GetIntersection(in float2 p0, in float2 p1, in float2 p2, in float2 p3, out float2 point)
		{
			// https://imagingsolution.blog.fc2.com/blog-entry-137.html

			float2 a0 = p1 - p0, a1 = p3 - p2;
			float2 b0 = p2 - p1, b1 = p0 - p2;

			if (math.abs(1.0 - math.abs(math.dot(math.normalize(a0), math.normalize(a1)))) < 1e-5)
			{
				point = new float2(float.NaN, float.NaN);
				return;
			}

			Cross(a1, b1, out var c0);
			Cross(a1, b0, out var c1);

			var s0 = c0 / 2.0f;
			var s1 = c1 / 2.0f;

			var t = s0 / (s0 + s1);
			var x = p0.x + a0.x * t;
			var y = p0.y + a0.y * t;

			point = new float2(x, y);
		}

		[BurstCompile]
		public static void GetForwardAndLeft(in float3 start, in float3 end, out float3 forward, out float3 left)
		{
			forward = math.normalize(end - start);
			left = new float3(-forward.y, forward.x, 0);
		}
	}
}
