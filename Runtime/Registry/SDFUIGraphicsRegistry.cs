using System.Collections.Generic;
using UnityEngine;

namespace TLab.UI.SDF.Registry
{
    internal class SDFUIGraphicsRegistry : MonoBehaviour
    {
        private static readonly List<SDFUI> _registry = new();

        internal static SDFUIGraphicsRegistry Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject gameObject = new($"[{nameof(SDFUIGraphicsRegistry)}]");
                    DontDestroyOnLoad(gameObject);
                    _instance = gameObject.AddComponent<SDFUIGraphicsRegistry>();
                }
                return _instance;
            }
        }
        private static SDFUIGraphicsRegistry _instance;

        [RuntimeInitializeOnLoadMethod]
        private static void Create()
        {
            var i = Instance;
        }

        private void Start()
        {
            if (this != _instance)
                Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (this == _instance)
                _registry.Clear();
        }

        private void LateUpdate()
        {
            foreach (SDFUI graphics in _registry)
            {
                graphics.OnLateUpdate();
            }
        }

        internal static void AddToRegistry(SDFUI graphics)
        {
            _registry.Add(graphics);
        }

        internal static void RemoveFromRegistry(SDFUI graphics)
        {
            _registry.Remove(graphics);
        }
    }
}
