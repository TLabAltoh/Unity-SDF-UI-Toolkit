using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public enum Shape
    {
        CIRCLE,
        BEZIER,
        NONE
    };

    public enum PreviewMode
    {
        PATH,
        SDF
    };

    [CreateAssetMenu(fileName = "SDF Tex Painter", menuName = "TLab/UI/SDF/SDF Tex Painter")]
    public class SDFTexPainter : ScriptableObject
    {
        [Range(1, 1024)] public int texWidth = 512;
        [Range(1, 1024)] public int texHeight = 512;
        [Range(1, 1024)] public int sdfWidth = 256;
        [Range(1, 1024)] public int sdfHeight = 256;

        public SDFSettings sdfSettings;

        public Shape shape = Shape.NONE;
        public PreviewMode previewMode = PreviewMode.PATH;

        public CirclePainter circlePainter;
        public BezierPainter bezierPainter;

        public Texture2D sdfTex;

        public string savePath = "";
    }
}
