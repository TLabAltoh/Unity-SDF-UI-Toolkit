using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.SDF.Registry
{
    internal static class MaterialRegistry
    {
        private static Dictionary<string, Shader> _shaiderCache = new();
        private static Dictionary<string, Stack<Material>> _materialsPool = new();
        private static Dictionary<MaterialRecord, (int usages, Material material)> _materialsInUse = new();
        private static Dictionary<SDFUI, MaterialRecord> _materialsByImage = new();


        public static void UpdateMaterial(SDFUI image)
        {
            if (_materialsByImage.TryGetValue(image, out MaterialRecord oldRecord))
                ReturnMaterial(oldRecord);
            MaterialRecord record = image.MaterialRecord;
            _materialsByImage[image] = record;
            var material = GetMaterial(record);
            image.material = material;
        }

        public static void StopUsingMaterial(SDFUI image)
        {
            if(_materialsByImage.TryGetValue(image, out MaterialRecord record))
            {
                ReturnMaterial(record);
                _materialsByImage.Remove(image);
            }
        }

        public static Material GetMaterial(MaterialRecord materialRecord)
        {
            Debug.Log(_materialsInUse.Count);
            if (_materialsInUse.TryGetValue(materialRecord, out var data))
            {
                data.usages++;
                return data.material;
            }
            Material material = GetNewMaterial(materialRecord.ShaiderName);
            material.name = materialRecord.GetHashCode().ToString();
            data = (1, material);
            materialRecord.Populate(data.material);
            _materialsInUse[materialRecord] = data;
            return data.material;
        }

        public static void ReturnMaterial(MaterialRecord materialRecord)
        {
            if (_materialsInUse.TryGetValue(materialRecord, out var data))
            {
                data.usages--;
                if (data.usages <= 0)
                {
                    _materialsInUse.Remove(materialRecord);
                    ReturnMaterial(materialRecord.ShaiderName, data.material);
                }
            }
        }

        private static Material GetNewMaterial(string shaider)
        {
            if (_materialsPool.TryGetValue(shaider, out var materials) && materials.Count > 0)
                return materials.Pop();
            if (!_shaiderCache.TryGetValue(shaider, out var unityShaider))
            {
                unityShaider = Shader.Find(shaider);
                _shaiderCache[shaider] = unityShaider;
            }
            return new Material(unityShaider);
        }

        private static void ReturnMaterial(string shaider, Material material)
        {
            if (!_materialsPool.TryGetValue(shaider, out var materials))
            {
                materials = new Stack<Material>();
                _materialsPool[shaider] = materials;
            }
            materials.Push(material);
        }
    }
}
