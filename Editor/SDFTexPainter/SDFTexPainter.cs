using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public enum PreviewMode
    {
        Path,
        SDF
    };

    [CreateAssetMenu(fileName = "SDF Tex Painter", menuName = "TLab/UI/SDF/SDF Tex Painter")]
    public class SDFTexPainter : ScriptableObject
    {
        [Min(0f)] public Vector2Int size = new Vector2Int(512, 512);
        [Min(0f)] public int texScale = 100;
        public SDFSettings sdfSettings;
        public PreviewMode previewMode = PreviewMode.Path;
        public BezierPainter bezierPainter;
        public Texture2D sdfTex;
        public string savePath = "";
    }
}
