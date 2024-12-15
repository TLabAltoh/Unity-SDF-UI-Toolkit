using System;
using UnityEditor;
using UnityEngine;
using static TLab.UI.SDF.SDFUI;

namespace TLab.UI.SDF
{
	public class SDFUISettings : ScriptableObject
	{
		private const string Path = "Assets/SDF-UI-Toolkit/Resources";
		private const float D9 = 217f / 255f;
		[SerializeField] private AntialiasingType _defaultAA = AntialiasingType.ON;
		[SerializeField] private bool _useOutline;
		[SerializeField, Min(0)] private float _outlineWidth = 1;
		[SerializeField] private Color _outlineColor = Color.black;
		[SerializeField] private OutlineType _outlineType = OutlineType.Inside;
		[SerializeField] private Color _fillColor = new(D9, D9, D9);
		[SerializeField] private bool _useShadow;
		[SerializeField] private Color _shadowColor = new(0, 0, 0, 0.25f);
		[SerializeField] private Vector2 _shadowOffset = new(0, 4);

		public AntialiasingType DefaultAA => _defaultAA;
		public bool UseOutline => _useOutline;
		public float OutlineWidth => _outlineWidth;
		public Color OutlineColor => _outlineColor;
		public OutlineType OutlineType => _outlineType;
		public Color FillColor => _fillColor;
		public bool UseShadow => _useShadow;
		public Color ShadowColor => _shadowColor;
		public Vector2 ShadowOffset => _shadowOffset;

#if UNITY_EDITOR
		public static event Action AASettingsChanged;
#endif

		public static SDFUISettings Instance
		{
			get
			{
				if (instance == null)
					instance = Resources.Load<SDFUISettings>(@"SDUISettings");

				if (instance == null)
				{
					instance = CreateInstance<SDFUISettings>();
#if UNITY_EDITOR
					if (!Application.isPlaying)
					{
						ValidateFolder(Path);
						AssetDatabase.CreateAsset(instance, Path + @"/SDUISettings.asset");
						AssetDatabase.Refresh();
					}
#endif
				}

				return instance;
			}
		}

		private AntialiasingType oldAA = AntialiasingType.Default;

		private static SDFUISettings instance;

#if UNITY_EDITOR
		private void OnValidate()
		{
			if (Application.isPlaying)
				return;
			if (oldAA is AntialiasingType.Default)
				oldAA = _defaultAA;
			if (_defaultAA != oldAA)
			{
				AASettingsChanged?.Invoke();
				oldAA = _defaultAA;
			}
		}

		private static void ValidateFolder(string path)
		{
			string[] folders = path.Split('/');
			string currentPath = folders[0];
			for (int i = 1; i < folders.Length; i++)
			{
				string nextPath = currentPath + @"/" + folders[i];
				if (!AssetDatabase.IsValidFolder(nextPath))
					AssetDatabase.CreateFolder(currentPath, folders[i]);
				currentPath = nextPath;
			}
		}
#endif
	}
}
