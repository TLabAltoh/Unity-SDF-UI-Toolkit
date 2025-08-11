using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace TLab.UI.SDF.Editor
{
	public static class SerializeUtil
	{
		public static string THIS_NAME => "[SerializeUtil] ";
		public static readonly GUIStyle style = new(EditorStyles.boldLabel)
		{
			fontSize = 16,
		};

		public static bool TryGetIntValue(this SerializedObject serializedObject, string name, out int result)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.intValue;
				return true;
			}
			result = 0;
			return false;
		}

		public static bool TryGetFloatValue(this SerializedObject serializedObject, string name, out float result)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.floatValue;
				return true;
			}
			result = 0f;
			return false;
		}

		public static bool TryGetBoolValue(this SerializedObject serializedObject, string name, out bool result)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.boolValue;
				return true;
			}
			result = false;
			return false;
		}

		public static bool TryGetEnumValue(this SerializedObject serializedObject, string name, out int result)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.enumValueIndex;
				return true;
			}
			result = default;
			return false;
		}

		public static bool TryGetStringValue(this SerializedObject serializedObject, string name, out string result)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.stringValue;
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryGetObject<T>(this SerializedObject serializedObject, string name, out T result) where T : UnityEngine.Object
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = (T)prop.objectReferenceValue;
				return result != null;
			}
			result = default;
			return false;
		}

		public static bool TryGetValue<T>(this SerializedObject serializedObject, string name, out T result) where T : class
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				result = prop.GetValue<T>();
				return result != null;
			}
			result = default;
			return false;
		}

		public static bool TryGetObjectList<T>(this SerializedObject serializedObject, string name, out List<T> result) where T : UnityEngine.Object
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null && prop.isArray)
			{
				result = new List<T>();
				for (int i = 0; i < prop.arraySize; i++)
					result.Add((T)prop.GetArrayElementAtIndex(i).objectReferenceValue);
				return true;
			}
			result = null;
			return false;
		}

		public static bool TryGetValueList<T>(this SerializedObject serializedObject, string name, out List<T> result) where T : class
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null && prop.isArray)
			{
				result = new List<T>();
				for (int i = 0; i < prop.arraySize; i++)
					result.Add(prop.GetArrayElementAtIndex(i).GetValue<T>());
				return true;
			}
			result = null;
			return false;
		}

		/// <summary>
		/// This function seems to need to call EditorUtility.SetDirty after calling this one to save the property change (But not sure, it is possible I missed something somewhere ...).
		/// In most cases, I prefer to replace it with serializeObject.Call(() => {@object.value = value; EditorUtility.SetDirty(@object)});
		/// </summary>
		/// <returns></returns>
		public static bool TrySetValue<T>(this SerializedObject serializedObject, string name, T value) where T : class
		{
			try
			{
				var prop = serializedObject.FindProperty(name);
				prop.SetValue(value);
				return true;
			}
			catch (Exception e)
			{
				Debug.LogError(THIS_NAME + e);
				throw;
			}
		}

		public static bool TryAddArrayElement(this SerializedObject serializedObject, string name, UnityEngine.Object @object)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				prop.InsertArrayElementAtIndex((prop.arraySize == 0) ? 0 : prop.arraySize - 1);
				var element = prop.GetArrayElementAtIndex(prop.arraySize - 1);
				element.objectReferenceValue = @object;
				return true;
			}
			return false;
		}

		public static bool TryAddArrayElement<T>(this SerializedObject serializedObject, string name, T value) where T : class
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				prop.InsertArrayElementAtIndex((prop.arraySize == 0) ? 0 : prop.arraySize - 1);
				var element = prop.GetArrayElementAtIndex(prop.arraySize - 1);
				element.SetValue(value);
				return true;
			}
			return false;
		}

		public static bool TryDrawProperty(this SerializedObject serializedObject, string name, string label)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				EditorGUILayout.PropertyField(prop, new GUIContent(label), true);
				return true;
			}
			return false;
		}

		public static bool TryDrawPropertyNoLabel(this SerializedObject serializedObject, string name)
		{
			var prop = serializedObject.FindProperty(name);

			if (prop != null)
			{
				float old = EditorGUIUtility.labelWidth;
				EditorGUIUtility.labelWidth = 0;
				EditorGUILayout.PropertyField(prop, new GUIContent(), true);
				EditorGUIUtility.labelWidth = old;
				return true;
			}

			return false;
		}

		public static bool TryDrawEnumProperty(this SerializedObject serializedObject, string name, string label, string[] options)
		{
			var prop = serializedObject.FindProperty(name);

			if (prop != null)
			{
				prop.enumValueIndex = EditorGUILayout.Popup(label, prop.enumValueIndex, options);

				return true;
			}

			return false;
		}

		public static bool TryDrawColorProperty(this SerializedObject serializedObject, string name, string label)
		{
			var prop = serializedObject.FindProperty(name);
			if (prop != null)
			{
				var color = prop.colorValue;
				color = EditorGUILayout.ColorField(new GUIContent(label), color, true, true, SDFUISettings.Instance.UseHDRColor);
				prop.colorValue = color;
				return true;
			}
			return false;
		}

		public static void Call(this SerializedObject serializedObject, UnityAction action)
		{
			serializedObject.ApplyModifiedProperties();
			action.Invoke();
			serializedObject.Update();
		}

		//
		// link: https://gist.github.com/douduck08/6d3e323b538a741466de00c30aa4b61f
		//

		private static readonly Regex rgx = new Regex(@"\[\d+\]", RegexOptions.Compiled);

		public static void QuickApply(this SerializedObject serializedObject)
		{
			serializedObject.ApplyModifiedProperties();
			serializedObject.Update();
		}

		public static T GetValue<T>(this SerializedProperty property) where T : class
		{
			property.serializedObject.QuickApply();

			var obj = (object)property.serializedObject.targetObject;
			var path = property.propertyPath.Replace(".Array.data", "");
			var fieldStructure = path.Split('.');
			for (int i = 0; i < fieldStructure.Length; i++)
			{
				if (fieldStructure[i].Contains("["))
				{
					var index = Convert.ToInt32(new string(fieldStructure[i].Where(c => char.IsDigit(c)).ToArray()));
					obj = GetFieldValueWithIndex(rgx.Replace(fieldStructure[i], ""), obj, index);
				}
				else
					obj = GetFieldValue(fieldStructure[i], obj);
			}
			return (T)obj;
		}

		public static bool SetValue<T>(this SerializedProperty property, T value) where T : class
		{
			// This Apply() is necessary to keep the current data consistent with the data accessed by SetValue()
			property.serializedObject.QuickApply();

			var obj = (object)property.serializedObject.targetObject;
			var path = property.propertyPath.Replace(".Array.data", "");
			var fieldStructure = path.Split('.');
			for (var i = 0; i < fieldStructure.Length - 1; i++)
			{
				if (fieldStructure[i].Contains("["))
				{
					var index = Convert.ToInt32(new string(fieldStructure[i].Where(c => char.IsDigit(c)).ToArray()));
					obj = GetFieldValueWithIndex(rgx.Replace(fieldStructure[i], ""), obj, index);
				}
				else
					obj = GetFieldValue(fieldStructure[i], obj);
			}

			var fieldName = fieldStructure.Last();
			if (fieldName.Contains("["))
			{
				var index = Convert.ToInt32(new string(fieldName.Where(c => char.IsDigit(c)).ToArray()));
				return SetFieldValueWithIndex(rgx.Replace(fieldName, ""), obj, index, value);
			}
			else
				return SetFieldValue(fieldName, obj, value);
		}

		private static object GetFieldValue(string fieldName, object obj, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			var field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
				return field.GetValue(obj);
			return default(object);
		}

		private static object GetFieldValueWithIndex(string fieldName, object obj, int index, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			var field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				var list = field.GetValue(obj);
				if (list.GetType().IsArray)
					return ((object[])list)[index];
				else if (list is IEnumerable)
					return ((IList)list)[index];
			}
			return default(object);
		}

		public static bool SetFieldValue(string fieldName, object obj, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			var field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				field.SetValue(obj, value);
				return true;
			}
			return false;
		}

		public static bool SetFieldValueWithIndex(string fieldName, object obj, int index, object value, bool includeAllBases = false, BindingFlags bindings = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
		{
			var field = obj.GetType().GetField(fieldName, bindings);
			if (field != null)
			{
				var list = field.GetValue(obj);
				if (list.GetType().IsArray)
				{
					((object[])list)[index] = value;
					return true;
				}
				else if (list is IEnumerable)
				{
					((IList)list)[index] = value;
					return true;
				}
			}
			return false;
		}
	}
}