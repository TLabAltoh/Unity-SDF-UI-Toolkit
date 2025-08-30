using Unity.Burst;
using Unity.Mathematics;

namespace TLab.UI.SDF
{
	[BurstCompile]
	public static class MeshUtils
	{
		[BurstCompile]
		public static void CalculateVertexes(in float2 rectSize, in float2 rectPivot, in float margin, in float2 shadowOffset, in float rotation, in SDFUI.AntialiasingType antialiasing,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			float3 pivotPoint = new(rectSize * rectPivot, 0);
			float4 shadowExpand = float4.zero;

			MathUtils.RotateVector(shadowOffset, rotation, out float2 rotatedOffset);

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

			if (antialiasing != SDFUI.AntialiasingType.OFF && rectSize.x > 0 && rectSize.y > 0)
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
		public static void GetQuadVertexData(in float2 rectSize, in float2 rectPoint, in float3 p0, in float3 p1, in float width,
			out VertexData vertex0, out VertexData vertex1, out VertexData vertex2, out VertexData vertex3)
		{
			var pivotPoint = new float3(rectSize * rectPoint, 0);

			MathUtils.GetForwardAndLeft(p0, p1, out var forward, out var left);

			vertex0 = new VertexData(p0 + left * width - pivotPoint, new float2(0, 0));
			vertex1 = new VertexData(p1 + left * width - pivotPoint, new float2(0, 1));
			vertex2 = new VertexData(p1 - left * width - pivotPoint, new float2(1, 1));
			vertex3 = new VertexData(p0 - left * width - pivotPoint, new float2(1, 0));
		}

		[BurstCompile]
		public static void ShadowSizeOffset(in float2 rectSize, in float2 shadowOffset, in float rotation, out float4 sizeOffset)
		{
			MathUtils.RotateVector(shadowOffset, rotation, out float2 rotatedOffset);
			sizeOffset = new float4(rotatedOffset / rectSize, rotatedOffset.x, rotatedOffset.y);
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

		public VertexData(float2 position, float2 uv)
		{
			this.position = new float3(position, 0);
			this.uv = uv;
		}
	}
}
