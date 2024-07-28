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
            if (!(obj is MaterialProperty property))
                return false;

            if (property.NameID != NameID)
                return false;

            if (property.Type != Type)
                return false;

            switch (Type)
            {
                case PropertyType.Int:
                    return property.IntValue == IntValue;
                case PropertyType.Float:
                    return property.FloatValue == FloatValue;
                case PropertyType.Vector:
                    return property.VectorValue.Equals(VectorValue);
                case PropertyType.Color:
                    return property.ColorValue == ColorValue;
                case PropertyType.Texture:
                    return property.TextureValue == TextureValue;
                default:
                    return false;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Type, NameID, IntValue, FloatValue, VectorValue, ColorValue, TextureValue);
        }
    }
}
