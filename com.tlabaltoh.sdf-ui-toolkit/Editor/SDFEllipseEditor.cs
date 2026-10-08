using UnityEditor;

namespace TLab.UI.SDF.Editor
{
	[CustomEditor(typeof(SDFEllipse), true)]
	[CanEditMultipleObjects]
	public class SDFEllipseEditor : SDFUIEditor
	{
		private SDFEllipse m_instance;

		protected override void OnEnable()
		{
			base.OnEnable();

			m_instance = target as SDFEllipse;
		}
	}
}