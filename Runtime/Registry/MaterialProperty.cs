using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TLab.UI.SDF.Registry
{
	internal struct MaterialProperty
	{
		internal enum PropertyType : byte
		{
			None,
			Int,
			Float,
			Vector,
			Color,
			Texture,
		}

		public PropertyType Type;
		public int NameID;
		public int IntValue;
		public float FloatValue;
		public float4 VectorValue;
		public Color ColorValue;
		public Texture TextureValue;

		public override bool Equals(object obj)
		{
			if (obj is not MaterialProperty property)
				return false;

			if (property.NameID != NameID)
				return false;

			if (property.Type != Type)
				return false;

			return Type switch
			{
				PropertyType.Int => property.IntValue == IntValue,
				PropertyType.Float => property.FloatValue == FloatValue,
				PropertyType.Vector => property.VectorValue.Equals(VectorValue),
				PropertyType.Color => property.ColorValue == ColorValue,
				PropertyType.Texture => property.TextureValue == TextureValue,
				_ => false,
			};
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(Type, NameID, IntValue, FloatValue, VectorValue, ColorValue, TextureValue);
		}

		internal struct Comparer : IComparer<MaterialProperty>
		{
			public int Compare(MaterialProperty x, MaterialProperty y)
			{
				return x.NameID.CompareTo(y.NameID);
			}
		}
	}
}
