using System.IO;
using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    public static class AssetUtil
    {
        public static string GetFileName(string path, bool ignore)
        {
            string fileName = Path.GetFileName(path);
            if (ignore)
            {
                string[] split = fileName.Split('.');
                if (split.Length > 1 && !split[split.Length - 1].Contains('\\') && !split[split.Length - 1].Contains('/'))
                {
                    return split[split.Length - 2];
                }
            }
            return fileName;
        }

        public static void SaveTexture(string savePath, ref Texture2D texture)
        {
            if (Path.GetExtension(savePath) != ".asset")
            {
                savePath += ".asset";
            }

            Texture2D asset = AssetDatabase.LoadAssetAtPath<Texture2D>(savePath);

            texture.name = GetFileName(savePath, true);

            if (asset != null)
            {
                asset.name = texture.name;
                EditorUtility.CopySerialized(texture, asset);
            }
            else
            {
                AssetDatabase.CreateAsset(texture, savePath);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("Save texture: " + savePath);
        }

        public static string GetDiskPath(string assetPath)
        {
            string assetDir = assetPath.Substring(0, assetPath.Length - 1 - Path.GetFileName(assetPath).Length);
            return Directory.GetCurrentDirectory() + "\\" + assetDir.Replace("/", "\\");
        }

        public static bool DirectoryExists(string assetPath)
        {
            if (assetPath.Length < "Assets".Length - 1)
            {
                return false;
            }

            return Directory.Exists(GetDiskPath(assetPath));
        }

        public static string GetInitialPath(string path)
        {
            return ((path != null) && DirectoryExists(path)) ? path : "Assets";
        }

        public static bool SelectSavePath(string currentPath, out string savePath)
        {
            string fullPath = EditorUtility.SaveFilePanel("Save Path", GetInitialPath(currentPath), "", "asset");
            if (fullPath == "")
            {
                savePath = currentPath;
                return false;
            }
            savePath = fullPath.Remove(0, Directory.GetCurrentDirectory().Length + 1);
            return true;
        }
    }
}
