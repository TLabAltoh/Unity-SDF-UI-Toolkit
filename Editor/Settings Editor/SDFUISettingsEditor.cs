using System;
using System.Collections.Generic;
using System.Linq;
using TLab.UI.SDF;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static TLab.UI.SDF.SDFUI;


public class SDFUISettingsEditor : EditorWindow
{
	[SerializeField] private TextAsset package;

	private SDFUISettings settings;
	private SerializedObject serializedObject;

	[MenuItem("TLab/UI/SDF/Settings")]
	public static void Create()
	{
		SDFUISettingsEditor wnd = GetWindow<SDFUISettingsEditor>(true, "SDFUISettings");
	}

	public void CreateGUI()
	{
		if (settings == null || serializedObject == null)
		{
			settings = SDFUISettings.Instance;
			serializedObject = new SerializedObject(settings);
		}

		VisualElement root = rootVisualElement;

		// Import UXML
		var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/Modules/Editor/Settings Editor/SDFUISettingsEditor.uxml");
		VisualElement uxml = visualTree.Instantiate();
		root.Add(uxml);

		DropdownField aa = root.Query<DropdownField>("aa-algorithm");
		SerializedProperty aaProperty = serializedObject.FindProperty("_defaultAA");

		InitDropDown(aa, aaProperty, AntialiasingType.Default, x => (AntialiasingType)x, x => (int)x);

		FloatField outlineWidth = root.Query<FloatField>("outline-width");
		outlineWidth.RegisterValueChangedCallback(x => outlineWidth.value = (outlineWidth.value < 0) ? 0 : outlineWidth.value);

		Label name = root.Query<Label>("package-name");
		Label version = root.Query<Label>("version");

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