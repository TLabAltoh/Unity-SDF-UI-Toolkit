using System;
using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.RoundedCorners
{
    [System.Serializable]
    public class Circle
    {
        public Vector2 center;
        public float radius;
    }

    [System.Serializable]
    public class Corner
    {
        public Vector2 position;
        public float radius;
    }

    [System.Serializable]
    public class Polygon
    {
        public Vector2 offset;
        public List<Corner> corners;
    }

    public enum EditMode
    {
        CIRCLE,
        POLYGON,
        NONE
    };

    public enum PreviewMode
    {
        VECTOR,
        RASTRIZED,
        SDF
    };

    [System.Serializable]
    public class RasterizeSettings
    {
        [SerializeField, Range(0.0f, 512.0f)] public float maxInside = 10.0f;
        [SerializeField, Range(0.0f, 512.0f)] public float maxOutside = 10.0f;
        [SerializeField, Range(0.0f, 512.0f)] public float postProcessDistance = 10.0f;
    }

    [System.Serializable]
    public class FXAASettings
    {
        [SerializeField, Range(0.0f, 1.0f)] public float fixedThreshold = 0.0312f;
        [SerializeField, Range(0.0f, 1.0f)] public float relativeThreshold = 0.063f;
        [SerializeField, Range(0.0f, 1.0f)] public float subpixelBlending = 0.75f;
    }

    [CreateAssetMenu()]
    public class SDFPolygon : ScriptableObject
    {
        [SerializeField, Range(1, 1024)] public int width = 512;
        [SerializeField, Range(1, 1024)] public int height = 512;
        [SerializeField, Range(1, 1024)] public int sdfWidth = 256;
        [SerializeField, Range(1, 1024)] public int sdfHeight = 256;

        [SerializeField] public RasterizeSettings rasterizeSettings;
        [SerializeField] public FXAASettings fxaaSettings;

        [SerializeField] public List<Circle> circles;
        [SerializeField] public List<Polygon> polygons;

        [SerializeField] public EditMode editMode = EditMode.NONE;
        [SerializeField] public PreviewMode previewMode = PreviewMode.VECTOR;

        [SerializeField, Range(0, 512)] public float radius = 50.0f;

        [SerializeField] public bool showAnchors = false;
        [SerializeField] public bool showSegments = false;

        [SerializeField] public Texture2D rasterizedTex;
        [SerializeField] public Texture2D sdfTex;

        [SerializeField] public string savePath = "";
    }
}