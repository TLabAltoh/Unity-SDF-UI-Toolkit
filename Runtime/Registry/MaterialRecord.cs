using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace TLab.UI.SDF.Registry
{
    internal class MaterialRecord : ICloneable
    {
        private readonly Dictionary<int, MaterialProperty> _properties = new();
        public string ShaiderName { get; set; }
        public Texture Texture { get; set; }
        public float4 TextureUV { get; set; }
        public Color TextureColor { get; set; }


        public void SetInteger(int propertyID, int value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Int,
                NameID = propertyID,
                IntValue = value,
            };
            SetProperty(property);
        }

        public void SetFloat(int propertyID, float value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Float,
                NameID = propertyID,
                FloatValue = value,
            };
            SetProperty(property);
        }

        public void SetVector(int propertyID, float4 value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Vector,
                NameID = propertyID,
                VectorValue = value,
            };
            SetProperty(property);
        }

        public void SetVector(int propertyID, float2 value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Vector,
                NameID = propertyID,
                VectorValue = new float4(value, 0, 0),
            };
            SetProperty(property);
        }

        public void SetColor(int propertyID, Color value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Color,
                NameID = propertyID,
                ColorValue = value,
            };
            SetProperty(property);
        }

        public void SetTexture(int propertyID, Texture value)
        {
            MaterialProperty property = new()
            {
                Type = MaterialProperty.PropertyType.Texture,
                NameID = propertyID,
                TextureValue = value,
            };
            SetProperty(property);
        }

        public void SetProperty(MaterialProperty property)
        {
            _properties[property.NameID] = property;
        }

        public void Populate(Material material)
        {
            material.mainTexture = Texture;
            material.mainTextureScale = new Vector2(TextureUV.z, TextureUV.w);
            material.mainTextureOffset = new Vector2(TextureUV.x, TextureUV.y);
            material.color = TextureColor;
            foreach (var property in _properties)
            {
                switch (property.Value.Type)
                {
                    case MaterialProperty.PropertyType.Int:
                        material.SetInt(property.Value.NameID, property.Value.IntValue);
                        break;
                    case MaterialProperty.PropertyType.Float:
                        material.SetFloat(property.Value.NameID, property.Value.FloatValue);
                        break;
                    case MaterialProperty.PropertyType.Vector:
                        material.SetVector(property.Value.NameID, property.Value.VectorValue);
                        break;
                    case MaterialProperty.PropertyType.Color:
                        material.SetColor(property.Value.NameID, property.Value.ColorValue);
                        break;
                    case MaterialProperty.PropertyType.Texture:
                        material.SetTexture(property.Value.NameID, property.Value.TextureValue);
                        break;
                    default:
                        break;
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is MaterialRecord record))
                return false;

            if (record._properties.Count != _properties.Count)
                return false;

            foreach (var property in _properties)
            {
                if (!record._properties.TryGetValue(property.Key, out var other))
                    return false;
                if (!property.Value.Equals(other))
                    return false;
            }
            return ShaiderName == record.ShaiderName &&
                   Texture == record.Texture &&
                   TextureUV.Equals(record.TextureUV) &&
                   TextureColor == record.TextureColor;
        }

        public override int GetHashCode()
        {
            int hc = 0;
            foreach (var p in _properties.Values.OrderBy(x => x.NameID))
            {
                hc ^= p.GetHashCode();
                hc = (hc << 7) | (hc >> (32 - 7)); //rotate hc to the left to swipe over all bits
            }
            return hc + HashCode.Combine(ShaiderName, Texture, TextureUV, TextureColor);
        }

        public object Clone()
        {
            MaterialRecord clone = new();
            clone.ShaiderName = ShaiderName;
            clone.Texture = Texture;
            clone.TextureUV = TextureUV;
            clone.TextureColor = TextureColor;
            foreach (var property in _properties)
                clone._properties[property.Key] = property.Value;
            return clone;
        }
    }
}
