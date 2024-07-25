using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFTex), true)]
	[CanEditMultipleObjects]
	public class SDFTexEditor : SDFUIEditor
	{
		private SDFTex m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFTex;
		}

		protected override void DrawCustomProp()
		{
			EditorGUI.indentLevel++;
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.radius), "Radius");
			serializedObject.TryDrawProperty("m_" + nameof(m_instance.sdfTexture), "SDFTexture");
			EditorGUI.indentLevel--;
		}
	}
}
