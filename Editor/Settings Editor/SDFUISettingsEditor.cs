using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static TLab.UI.SDF.SDFUI;

namespace TLab.UI.SDF.Editor
{
	public class SDFUISettingsEditor : EditorWindow
	{
		private SDFUISettings settings;
		private SerializedObject serializedObject;

		private const string PACKAGE_NAME = "com.tlabaltoh.sdf-ui-toolkit";

		// Asset path is used in development phase (if this package is not installed by upm)
		private static string PACKAGE_PATH => IsThisPackage() ? $"Packages/{PACKAGE_NAME}" : "Assets/TLab/Unity-SDF-UI-Toolkit";

		[MenuItem("TLab/UI/SDF/Settings")]
		public static void Create()
		{
			SDFUISettingsEditor wnd = GetWindow<SDFUISettingsEditor>(true, "SDFUISettings");
		}

		public static bool IsPackageInstalled(string package)
		{
			string jsonText = System.IO.File.ReadAllText("Packages/manifest.json");

			JToken json = JToken.Parse(jsonText);

			var dependencies = json["dependencies"];
			return dependencies[package] != null;
		}

		public static bool IsThisPackage()
		{
			return IsPackageInstalled(PACKAGE_NAME);
		}

		public void CreateGUI()
		{
			settings = SDFUISettings.Instance;
			serializedObject = new SerializedObject(settings);

			VisualElement root = rootVisualElement;

			// Import UXML
			var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>($"{PACKAGE_PATH}/Editor/Settings Editor/SDFUISettingsEditor.uxml");
			VisualElement uxml = visualTree.Instantiate();
			root.Add(uxml);

			DropdownField aa = root.Query<DropdownField>("aa-algorithm");
			SerializedProperty aaProperty = serializedObject.FindProperty("_defaultAA");

			InitDropDown(aa, aaProperty, AntialiasingType.Default, x => (AntialiasingType)x, x => (int)x);

			FloatField outlineWidth = root.Query<FloatField>("outline-width");
			outlineWidth.RegisterValueChangedCallback(x => outlineWidth.value = (outlineWidth.value < 0) ? 0 : outlineWidth.value);

			Label name = root.Query<Label>("package-name");
			Label version = root.Query<Label>("version");

			var package = AssetDatabase.LoadAssetAtPath<TextAsset>($"{PACKAGE_PATH}/package.json");

			JToken packageData = JToken.Parse(package.text);
			version.text = packageData["version"].ToString();
			name.text = packageData["displayName"].ToString();

			OnPlayModeChanged();
			EditorApplication.playModeStateChanged += OnPlayModeChanged;

			root.Bind(serializedObject);
		}

		private void OnDestroy()
		{
			EditorApplication.playModeStateChanged -= OnPlayModeChanged;
		}

		private void InitDropDown<T>(DropdownField field, SerializedProperty property, T exclude, Func<int, T> fromInt, Func<T, int> toInt)
		{
			List<T> variants = new();
			foreach (T variant in Enum.GetValues(typeof(T)))
			{
				if (variant is AntialiasingType.Default)
					continue;
				variants.Add(variant);
			}
			field.choices = variants.Select(x => ObjectNames.NicifyVariableName(x.ToString())).ToList();
			field.index = variants.IndexOf(fromInt(property.intValue));
			field.RegisterValueChangedCallback(evt =>
			{
				if (field.index < 0 || field.index >= variants.Count)
					return;
				property.intValue = toInt(variants[field.index]);
				serializedObject.ApplyModifiedProperties();
			});
		}

		private void OnPlayModeChanged(PlayModeStateChange change)
		{
			OnPlayModeChanged();
		}

		private void OnPlayModeChanged()
		{
			GroupBox settingsGroup = rootVisualElement.Query<GroupBox>("settings");
			settingsGroup.SetEnabled(!EditorApplication.isPlaying);
			settingsGroup.style.opacity = EditorApplication.isPlaying ? 0.5f : 1;
		}
	}
}