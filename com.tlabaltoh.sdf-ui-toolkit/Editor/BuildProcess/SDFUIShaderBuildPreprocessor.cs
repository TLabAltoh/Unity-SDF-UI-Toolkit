using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.Rendering;
using PackageManagerInfo = UnityEditor.PackageManager.PackageInfo;

namespace TLab.UI.SDF.Editor
{
	internal sealed class SDFUIShaderBuildPreprocessor : IPreprocessBuildWithReport
	{
		private const string LiquidGlassPackageName = "com.tlabaltoh.sdf-ui-toolkit-liquidglass";
		private const string AssetsRoot = "Assets";
		private const string StartupSessionKey = "TLab.UI.SDF.AlwaysIncludedShadersUpdated";

		private static readonly PropertyInfo ShaderNameProperty = typeof(SDFUI).GetProperty(
			"SHADER_NAME",
			BindingFlags.Instance | BindingFlags.NonPublic);

		private static readonly PropertyInfo ShaderTypeProperty = typeof(SDFUI).GetProperty(
			"SHADER_TYPE",
			BindingFlags.Instance | BindingFlags.NonPublic);

		public int callbackOrder => 0;

		public void OnPreprocessBuild(BuildReport report)
		{
			UpdateAlwaysIncludedShaders(true);
		}

		[MenuItem("TLab/UI/SDF/Update Always Included Shaders")]
		internal static void UpdateFromEditor()
		{
			try
			{
				UpdateAlwaysIncludedShaders(false);
				RefreshLoadedSDFUIGraphics();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		[InitializeOnLoadMethod]
		private static void ScheduleStartupUpdate()
		{
			if (Application.isBatchMode || SessionState.GetBool(StartupSessionKey, false))
				return;

			SessionState.SetBool(StartupSessionKey, true);
			EditorApplication.delayCall += UpdateWhenEditorIsReady;
		}

		private static void UpdateWhenEditorIsReady()
		{
			if (EditorApplication.isCompiling || EditorApplication.isUpdating)
			{
				EditorApplication.delayCall += UpdateWhenEditorIsReady;
				return;
			}

			UpdateFromEditor();
		}

		private static void UpdateAlwaysIncludedShaders(bool throwOnError)
		{
			ClearExistingSDFUIShaders();

			var referencedTypes = FindReferencedSDFUITypes();
			if (referencedTypes.Count == 0)
			{
				Debug.Log("SDF UI shader check found no referenced SDF UI component types.");
				return;
			}

			var includeLiquidGlass = IsPackageInstalled(LiquidGlassPackageName);
			var errors = new List<string>();
			var shaders = ResolveShaders(referencedTypes, includeLiquidGlass, errors);
			var addedShaders = AddToAlwaysIncludedShaders(shaders);

			var shaderNames = string.Join(", ", shaders.Select(shader => shader.name));
			Debug.Log(
				$"SDF UI shader check found {referencedTypes.Count} referenced component type(s). " +
				$"Verified {shaders.Count} shader(s): {shaderNames}. " +
				$"Added {addedShaders} shader(s) to Always Included Shaders.");

			if (errors.Count == 0)
				return;

			var errorMessage = "SDF UI shader check failed:\n" + string.Join("\n", errors);
			if (throwOnError)
				throw new BuildFailedException(errorMessage);

			Debug.LogError(errorMessage);
		}

		private static void ClearExistingSDFUIShaders()
		{
			var graphicsSettings = GraphicsSettings.GetGraphicsSettings();
			var serializedSettings = new SerializedObject(graphicsSettings);
			var alwaysIncludedShaders = serializedSettings.FindProperty("m_AlwaysIncludedShaders");

			if (alwaysIncludedShaders == null || !alwaysIncludedShaders.isArray)
				throw new BuildFailedException("Always Included Shaders could not be found in Graphics Settings.");

			const string sdfShaderPrefix = "Hidden/UI/SDF/";

			for (int i = alwaysIncludedShaders.arraySize - 1; i >= 0; i--)
			{
				var shader = alwaysIncludedShaders.GetArrayElementAtIndex(i).objectReferenceValue as Shader;
				if (shader == null)
					continue;
				if (shader.name.StartsWith(sdfShaderPrefix, StringComparison.Ordinal))
					alwaysIncludedShaders.DeleteArrayElementAtIndex(i);
			}

			serializedSettings.ApplyModifiedPropertiesWithoutUndo();
			EditorUtility.SetDirty(graphicsSettings);
			AssetDatabase.SaveAssets();
		}

		private static List<Type> FindReferencedSDFUITypes()
		{
			var derivedTypes = new HashSet<Type>(
				TypeCache.GetTypesDerivedFrom<SDFUI>()
					.Where(type => !type.IsAbstract && typeof(MonoBehaviour).IsAssignableFrom(type)));

			if (derivedTypes.Count == 0)
				return new List<Type>();

			var scriptPathsByType = MonoImporter.GetAllRuntimeMonoScripts()
				.Select(script => new
				{
					Type = script.GetClass(),
					Path = AssetDatabase.GetAssetPath(script)
				})
				.Where(entry => entry.Type != null &&
								derivedTypes.Contains(entry.Type) &&
								!string.IsNullOrEmpty(entry.Path))
				.GroupBy(entry => entry.Type)
				.ToDictionary(group => group.Key, group => group.Select(entry => entry.Path).ToArray());

			var assetPaths = FindProjectGameObjectAssetPaths();
			if (assetPaths.Length == 0)
				return new List<Type>();

			var dependencies = new HashSet<string>(
				AssetDatabase.GetDependencies(assetPaths, true),
				StringComparer.OrdinalIgnoreCase);

			return scriptPathsByType
				.Where(pair => pair.Value.Any(dependencies.Contains))
				.Select(pair => pair.Key)
				.OrderBy(type => type.FullName, StringComparer.Ordinal)
				.ToList();
		}

		private static string[] FindProjectGameObjectAssetPaths()
		{
			return AssetDatabase.FindAssets("t:Prefab", new[] { AssetsRoot })
				.Concat(AssetDatabase.FindAssets("t:Scene", new[] { AssetsRoot }))
				.Select(AssetDatabase.GUIDToAssetPath)
				.Where(path => !string.IsNullOrEmpty(path))
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.ToArray();
		}

		private static List<Shader> ResolveShaders(
			IEnumerable<Type> componentTypes,
			bool includeLiquidGlass,
			ICollection<string> errors)
		{
			if (ShaderNameProperty == null || ShaderTypeProperty == null)
				throw new BuildFailedException("SDF UI shader properties could not be found.");

			var shadersByName = new SortedDictionary<string, Shader>(StringComparer.Ordinal);

			foreach (var componentType in componentTypes)
			{
				TryAddResolvedShader(componentType, "Default", shadersByName, errors);

				if (includeLiquidGlass)
					TryAddResolvedShader(componentType, "LiquidGlass", shadersByName, errors);
			}

			return shadersByName.Values.ToList();
		}

		private static void TryAddResolvedShader(
			Type componentType,
			string shaderType,
			IDictionary<string, Shader> shadersByName,
			ICollection<string> errors)
		{
			try
			{
				var shaderName = GetShaderName(componentType, shaderType);
				if (string.IsNullOrWhiteSpace(shaderName))
				{
					errors.Add(
						$"SDF UI component '{componentType.FullName}' returned an empty SHADER_NAME for '{shaderType}'.");
					return;
				}

				var shader = Shader.Find(shaderName);
				if (shader == null)
				{
					errors.Add(
						$"SDF UI component '{componentType.FullName}' references missing shader '{shaderName}'.");
					return;
				}

				shadersByName[shaderName] = shader;
			}
			catch (Exception exception)
			{
				errors.Add(
					$"Could not resolve SHADER_NAME for SDF UI component '{componentType.FullName}' " +
					$"and shader type '{shaderType}': {exception.Message}");
			}
		}

		private static string GetShaderName(Type componentType, string shaderType)
		{
			var gameObject = new GameObject($"SDF UI shader probe ({componentType.Name})")
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.SetActive(false);

			try
			{
				var component = gameObject.AddComponent(componentType) as SDFUI;
				if (component == null)
					throw new BuildFailedException($"Could not create SDF UI component '{componentType.FullName}'.");

				ShaderTypeProperty.SetValue(component, shaderType);
				return ShaderNameProperty.GetValue(component) as string;
			}
			catch (TargetInvocationException exception)
			{
				throw new BuildFailedException(
					$"Could not resolve SHADER_NAME for SDF UI component '{componentType.FullName}': " +
					$"{exception.InnerException?.Message ?? exception.Message}");
			}
			finally
			{
				UnityEngine.Object.DestroyImmediate(gameObject);
			}
		}

		private static int AddToAlwaysIncludedShaders(IReadOnlyCollection<Shader> shaders)
		{
			var graphicsSettings = GraphicsSettings.GetGraphicsSettings();
			var serializedSettings = new SerializedObject(graphicsSettings);
			var alwaysIncludedShaders = serializedSettings.FindProperty("m_AlwaysIncludedShaders");

			if (alwaysIncludedShaders == null || !alwaysIncludedShaders.isArray)
				throw new BuildFailedException("Always Included Shaders could not be found in Graphics Settings.");

			var existingShaders = new HashSet<Shader>();
			for (var index = 0; index < alwaysIncludedShaders.arraySize; index++)
			{
				var shader = alwaysIncludedShaders.GetArrayElementAtIndex(index).objectReferenceValue as Shader;
				if (shader != null)
					existingShaders.Add(shader);
			}

			var addedCount = 0;
			foreach (var shader in shaders)
			{
				if (!existingShaders.Add(shader))
					continue;

				var newIndex = alwaysIncludedShaders.arraySize;
				alwaysIncludedShaders.InsertArrayElementAtIndex(newIndex);
				alwaysIncludedShaders.GetArrayElementAtIndex(newIndex).objectReferenceValue = shader;
				addedCount++;
			}

			if (addedCount > 0)
			{
				serializedSettings.ApplyModifiedPropertiesWithoutUndo();
				EditorUtility.SetDirty(graphicsSettings);
				AssetDatabase.SaveAssets();
			}

			return addedCount;
		}

		private static void RefreshLoadedSDFUIGraphics()
		{
			foreach (var sdfUI in Resources.FindObjectsOfTypeAll<SDFUI>())
			{
				if (!EditorUtility.IsPersistent(sdfUI))
					sdfUI.SetMaterialDirty();
			}

			Canvas.ForceUpdateCanvases();
			SceneView.RepaintAll();
		}

		private static bool IsPackageInstalled(string packageName)
		{
			return PackageManagerInfo.GetAllRegisteredPackages()
				.Any(package => string.Equals(package.name, packageName, StringComparison.Ordinal));
		}
	}
}
