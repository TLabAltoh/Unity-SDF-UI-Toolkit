using System;
using System.Linq;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

namespace TLab.UI.SDF.Registry
{
	internal class MaterialRecord : ICloneable
	{
		private static readonly MaterialProperty.Comparer propertieComparer = new();
		private readonly Dictionary<int, MaterialProperty> _properties = new();

		private static readonly MaterialKeyword.Comparer keywordComparer = new();
		private readonly Dictionary<string, MaterialKeyword> _keywords = new();

		public string[] EnableKeywords => _keywords.Where(k => k.Value.Active).Select(k => k.Value.Keyword).ToArray();

		public string[] DisableKeywords => _keywords.Where(k => !k.Value.Active).Select(k => k.Value.Keyword).ToArray();

		public string ShaderName { get; set; }
		public Texture Texture { get; set; }
		public float4 TextureUV { get; set; }
		public Color TextureColor { get; set; }
		public GraphicsBuffer Buffer { get; set; }


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

		public void SetBuffer(int propertyID, GraphicsBuffer buffer)
        {
			MaterialProperty property = new()
			{
				Type = MaterialProperty.PropertyType.Buffer,
				NameID = propertyID,
				Buffer = buffer,
			};
			SetProperty(property);
        }

		public void SetKeywordActive(string keyword, bool active)
		{
			MaterialKeyword materialKeyword = new()
			{
				Keyword = keyword,
				Active = active
			};
			_keywords[keyword] = materialKeyword;
		}

		public void EnableKeyword(string keyword)
		{
			SetKeywordActive(keyword, true);
		}

		public void EnableKeyword(params string[] keywords)
		{
			foreach (var keyword in keywords)
				EnableKeyword(keyword);
		}

		public void DisableKeyword(string keyword)
		{
			SetKeywordActive(keyword, false);
		}

		public void DisableKeyword(params string[] keywords)
		{
			foreach (var keyword in keywords)
				DisableKeyword(keyword);
		}

		public void SetProperty(MaterialProperty property)
		{
			_properties[property.NameID] = property;
		}

		public bool NeedsRecompile(Material material)
		{
			var enables = new SortedSet<string>(material.enabledKeywords.Select(k => k.name));
			return enables.Intersect(DisableKeywords).Any() || !(new SortedSet<string>(EnableKeywords).IsSubsetOf(enables));
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
					case MaterialProperty.PropertyType.Buffer:
						material.SetBuffer(property.Value.NameID, property.Value.Buffer);
						break;
					default:
						break;
				}
			}

			if (NeedsRecompile(material))
			{
				foreach (var keyword in _keywords)
					material.SetKeyword(new LocalKeyword(material.shader, keyword.Value.Keyword), keyword.Value.Active);
			}
		}

		public override bool Equals(object obj)
		{
			if (obj is not MaterialRecord record)
				return false;

			// Properties
			if (record._properties.Count != _properties.Count)
				return false;

			foreach (var property in _properties)
			{
				if (!record._properties.TryGetValue(property.Key, out var other))
					return false;
				if (!property.Value.Equals(other))
					return false;
			}

			// Keywords
			if (record._keywords.Count != _keywords.Count)
				return false;

			foreach (var keyword in _keywords)
			{
				if (!record._keywords.TryGetValue(keyword.Key, out var other))
					return false;
				if (!keyword.Value.Equals(other))
					return false;
			}

			// Others
			return ShaderName == record.ShaderName &&
				   Texture == record.Texture &&
				   TextureUV.Equals(record.TextureUV) &&
				   TextureColor == record.TextureColor &&
				   Buffer == record.Buffer;
		}

		public override int GetHashCode()
		{
			int hc = 0;

			SortedSet<MaterialProperty> properties = new(_properties.Values, propertieComparer);
			foreach (var p in properties)
			{
				hc ^= p.GetHashCode();
				hc = (hc << 7) | (hc >> (32 - 7)); //rotate hc to the left to swipe over all bits
			}

			SortedSet<MaterialKeyword> keywords = new(_keywords.Values, keywordComparer);
			foreach (var p in keywords)
			{
				hc ^= p.GetHashCode();
				hc = (hc << 7) | (hc >> (32 - 7));
			}

			return hc + HashCode.Combine(ShaderName, Texture, TextureUV, TextureColor, Buffer);
		}

		public object Clone()
		{
			MaterialRecord clone = new();
			clone.ShaderName = ShaderName;
			clone.Texture = Texture;
			clone.TextureUV = TextureUV;
			clone.TextureColor = TextureColor;
			clone.Buffer = Buffer;

			foreach (var property in _properties)
				clone._properties[property.Key] = property.Value;

			foreach (var keyword in _keywords)
				clone._keywords[keyword.Key] = keyword.Value;

			return clone;
		}
	}
}
