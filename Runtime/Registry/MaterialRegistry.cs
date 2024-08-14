using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.SDF.Registry
{
	internal static class MaterialRegistry
	{
		private static Dictionary<string, Shader> _shaderCache = new();
		private static Dictionary<string, Stack<Material>> _materialsPool = new();
		private static Dictionary<MaterialRecord, MaterialUsage> _materialsInUse = new();
		private static Dictionary<SDFUI, MaterialRecord> _materialsByImage = new();

		static MaterialRegistry()
		{
#if UNITY_EDITOR
			SDFUISettings.AASettingsChanged += OnSettingsChanged;
#endif
		}

		public static void UpdateMaterial(SDFUI image)
		{
			MaterialRecord record = image.MaterialRecord;
			if (_materialsByImage.TryGetValue(image, out MaterialRecord oldRecord))
			{
				if (_materialsInUse.TryGetValue(oldRecord, out var data))
				{
					if (data.Usages == 1 && record.ShaderName == oldRecord.ShaderName && !_materialsInUse.ContainsKey(record))
					{
						_materialsInUse.Remove(oldRecord);
						_materialsInUse[record] = data;
						_materialsByImage[image] = record;
						record.Populate(data.Material);
						if (image.material != data.Material)
							image.material = data.Material;
						return;
					}
					else
					{
						ReturnMaterial(oldRecord);
					}
				}
			}
			_materialsByImage[image] = record;
			var material = GetMaterial(record);
			if (image.material != material)
				image.material = material;
		}

		public static void StopUsingMaterial(SDFUI image)
		{
			if (_materialsByImage.TryGetValue(image, out MaterialRecord record))
			{
				ReturnMaterial(record);
				_materialsByImage.Remove(image);
			}
		}

		private static Material GetMaterial(MaterialRecord materialRecord)
		{
			if (_materialsInUse.TryGetValue(materialRecord, out var data))
			{
				data.Usages++;
				return data.Material;
			}
			Material material = GetNewMaterial(materialRecord.ShaderName);
			material.name = $"{materialRecord.GetHashCode()}";
			data = new MaterialUsage(material);
			materialRecord.Populate(data.Material);
			_materialsInUse[materialRecord] = data;
			return data.Material;
		}

		private static void ReturnMaterial(MaterialRecord materialRecord)
		{
			int hash = materialRecord.GetHashCode();
			if (_materialsInUse.TryGetValue(materialRecord, out var data))
			{
				data.Usages--;
				if (data.Usages <= 0)
				{
					_materialsInUse.Remove(materialRecord);
					ReturnMaterial(materialRecord.ShaderName, data.Material);
				}
			}
		}

		private static Material GetNewMaterial(string shader)
		{
			if (_materialsPool.TryGetValue(shader, out var materials) && materials.Count > 0)
			{
				var material = materials.Pop();
				if (material != null)
					return material;
			}
			if (!_shaderCache.TryGetValue(shader, out var unityShader))
			{
				unityShader = Shader.Find(shader);
				_shaderCache[shader] = unityShader;
			}
			return new Material(unityShader);
		}

		private static void ReturnMaterial(string shader, Material material)
		{
			if (!_materialsPool.TryGetValue(shader, out var materials))
			{
				materials = new Stack<Material>();
				_materialsPool[shader] = materials;
			}
			materials.Push(material);
		}

		private static void LogMaterialsInUse() //For Debugging in future. DO NOT REMOVE!
		{
			string log = $"{_materialsInUse.Count}\n";
			foreach (var materialPair in _materialsInUse)
			{
				log += $"{materialPair.Key.GetHashCode()}: {materialPair.Value}\n";
			}
			Debug.Log(log);
		}

		private static void OnSettingsChanged()
		{
			foreach (var materialPair in _materialsByImage)
				materialPair.Key.SetMaterialDirty();
		}

		private class MaterialUsage
		{
			public int Usages { get; set; }
			public Material Material { get; }


			public MaterialUsage(Material material) : this(1, material)
			{
			}

			public MaterialUsage(int usages, Material material)
			{
				Usages = usages;
				Material = material;
			}

			public override string ToString()
			{
				return $"[{Usages}, {Material}]";
			}
		}
	}
}
