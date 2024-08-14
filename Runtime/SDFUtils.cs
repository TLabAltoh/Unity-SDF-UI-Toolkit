using Unity.Burst;
using Unity.Mathematics;

namespace TLab.UI.SDF
{
	[BurstCompile]
	public static class SDFUtils
	{
		[BurstCompile]
		public static void CalculateVertexes(in float2 rectSize, in float2 rectPivot, in float margin, in float2 shadowOffset, in float rotation, in SDFUI.AntialiasingType antialiasing,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			float3 pivotPoint = new(rectSize * rectPivot, 0);
			float4 shadowExpand = float4.zero;

			RotateVector(shadowOffset, rotation, out float2 rotatedOffset);

			if (rotatedOffset.x < 0)
			{
				shadowExpand.x = rotatedOffset.x;
				shadowExpand.y = 0;
			}
			else
			{
				shadowExpand.x = 0;
				shadowExpand.y = rotatedOffset.x;
			}

			if (rotatedOffset.y < 0)
			{
				shadowExpand.z = rotatedOffset.y;
				shadowExpand.w = 0;
			}
			else
			{
				shadowExpand.z = 0;
				shadowExpand.w = rotatedOffset.y;
			}

			float4 expand = shadowExpand;

			if (antialiasing != SDFUI.AntialiasingType.None && rectSize.x > 0 && rectSize.y > 0)
			{
				expand += new float4(-1, 1, -1, 1);
			}

			float scaleX = math.mad(2, margin, rectSize.x);
			float scaleY = math.mad(2, margin, rectSize.y);
			float4 uvExpand = new(expand.x / scaleX, expand.y / scaleX, expand.z / scaleY, expand.w / scaleY);

			vertex0 = new VertexData();
			vertex0.position = new float3(expand.x - margin, expand.z - margin, 0) - pivotPoint;
			vertex0.uv = new float2(uvExpand.x, uvExpand.z);

			vertex1 = new VertexData();
			vertex1.position = new float3(expand.x - margin, expand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex1.uv = new float2(uvExpand.x, 1 + uvExpand.w);

			vertex2 = new VertexData();
			vertex2.position = new float3(expand.y + margin + rectSize.x, expand.w + margin + rectSize.y, 0) - pivotPoint;
			vertex2.uv = new float2(1 + uvExpand.y, 1 + uvExpand.w);

			vertex3 = new VertexData();
			vertex3.position = new float3(expand.y + margin + rectSize.x, expand.z - margin, 0) - pivotPoint;
			vertex3.uv = new float2(1 + uvExpand.y, uvExpand.z);
		}

		[BurstCompile]
		public static void QuadraticBezier(in float2 p0, in float2 p1, in float2 p2, in float t, out float2 point)
		{
			point = (1 - t) * (1 - t) * p0 + 2 * (1 - t) * p1 + t * t * p2;
		}

		[BurstCompile]
		public static void QuadraticBezierTangent(in float2 p0, in float2 p1, in float2 p2, in float t, out float2 tangent)
		{
			tangent = 2 * t * (p2 - p0) + 2 * (p0 + p1);
		}

		[BurstCompile]
		public static void GetQuadraticBezierVertexData(in float2 rectSize, in float2 rectPoint, in float3 p0, in float3 p1, in float3 p2, in float width,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3, out VertexData vertex4, out VertexData vertex5, out VertexData vertex6, out VertexData vertex7)
		{
			var pivotPoint = new float3(rectSize * rectPoint, 0);

			GetForwardAndLeft(p0, p1, out var forward0, out var left0);
			GetForwardAndLeft(p1, p2, out var forward1, out var left1);

			vertex0 = new VertexData(p0 + left0 * width);
			vertex1 = new VertexData(p0 - left0 * width);
			vertex6 = new VertexData(p2 + left1 * width);
			vertex7 = new VertexData(p2 - left1 * width);

			QuadraticBezierTangent(p0.xy, p1.xy, p2.xy, 0.5f, out var tangent);
			var normal = new float2(-tangent.y, tangent.x);
			QuadraticBezier(p0.xy, p1.xy, p2.xy, 0.5f, out var m);

			GetIntersection(m.xy + normal * width, tangent.xy, vertex0.position.xy, forward0.xy, out var intersection0);
			GetIntersection(m.xy - normal * width, tangent.xy, vertex1.position.xy, forward1.xy, out var intersection1);
			GetIntersection(m.xy + normal * width, tangent.xy, vertex6.position.xy, forward0.xy, out var intersection2);
			GetIntersection(m.xy - normal * width, tangent.xy, vertex7.position.xy, forward1.xy, out var intersection3);

			vertex2 = new VertexData(intersection0);
			vertex3 = new VertexData(intersection1);
			vertex4 = new VertexData(intersection2);
			vertex5 = new VertexData(intersection3);

			vertex0.position -= pivotPoint;
			vertex1.position -= pivotPoint;
			vertex2.position -= pivotPoint;
			vertex3.position -= pivotPoint;
			vertex4.position -= pivotPoint;
			vertex5.position -= pivotPoint;
			vertex6.position -= pivotPoint;
			vertex7.position -= pivotPoint;
		}

		[BurstCompile]
		public static void GetQuadVertexData(in float2 rectSize, in float2 rectPoint, in float3 p0, in float3 p1, in float width,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			var pivotPoint = new float3(rectSize * rectPoint, 0);

			GetForwardAndLeft(p0, p1, out var forward, out var left);

			vertex0 = new VertexData(p0 + left * width - pivotPoint, new float2(0, 0));
			vertex1 = new VertexData(p1 + left * width - pivotPoint, new float2(0, 1));
			vertex2 = new VertexData(p1 - left * width - pivotPoint, new float2(1, 1));
			vertex3 = new VertexData(p0 - left * width - pivotPoint, new float2(1, 0));
		}

		[BurstCompile]
		public static void ShadowSizeOffset(in float2 rectSize, in float2 shadowOffset, in float rotation, out float4 sizeOffset)
		{
			RotateVector(shadowOffset, rotation, out float2 rotatedOffset);
			sizeOffset = new float4(rotatedOffset / rectSize, rotatedOffset.x, rotatedOffset.y);
		}

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
		public static void GetVectorDiff(in float3 p0, in float3 p1, out float3 diff, out float3 dir, out float len)
		{
			diff = p1 - p0;
			dir = math.normalize(diff);
			len = math.length(diff);
		}

		[BurstCompile]
		public static void GetIntersection(in float2 p0, in float2 dir0, in float2 p1, in float2 dir1, out float2 point)
		{
			if (math.distance(p0, p1) == 0)
			{
				point = p0;
				return;
			}

			if (math.abs(1.0 - math.abs(math.dot(dir0, dir1))) < 1e-5)
			{
				point = (p0 + p1) * 0.5f;
				return;
			}

			var grad0 = dir0.y / dir0.x;
			var grad1 = dir1.y / dir1.x;

			var intercept0 = p0.y - p0.x * grad0;
			var intercept1 = p1.y - p1.x * grad1;

			point = new float2();
			point.x = (intercept1 - intercept0) / (grad0 - grad1);
			point.y = point.x * grad0 + intercept0;
		}

		[BurstCompile]
		public static void GetForwardAndLeft(in float3 start, in float3 end, out float3 forward, out float3 left)
		{
			forward = math.normalize(end - start);
			left = new float3(-forward.y, forward.x, 0);
		}
	}

	public struct VertexData
	{
		public float3 position;
		public float2 uv;

		public VertexData(float3 position, float2 uv)
		{
			this.position = position;
			this.uv = uv;
		}

		public VertexData(float3 position)
		{
			this.position = position;
			this.uv = new float2();
		}

		public VertexData(float2 uv)
		{
			this.position = new float3();
			this.uv = uv;
		}
	}
}
