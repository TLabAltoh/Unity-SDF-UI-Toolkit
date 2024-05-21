using System;
using System.Reflection;
using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    /// <summary>
    /// https://forum.unity.com/threads/extend-editorgui.465968/
    /// </summary>
    public static class GUIView
    {

        private static Type guiViewType;
        private static MethodInfo sendEvent;
        private static MethodInfo repaintInfo;
        private static PropertyInfo currentInfo;
        private static MethodInfo repaintImmediatelyInfo;
        private static object[] sendEventParams;

        public static object Current
        {

            get { return currentInfo.GetValue(null, null); }
        }

        static GUIView()
        {

            guiViewType = Type.GetType("UnityEditor.GUIView, UnityEditor");

            sendEvent = guiViewType.GetMethod("SendEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            currentInfo = guiViewType.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            repaintInfo = guiViewType.GetMethod("Repaint", BindingFlags.Public | BindingFlags.Instance);
            repaintImmediatelyInfo = guiViewType.GetMethod("RepaintImmediately", BindingFlags.Public | BindingFlags.Instance);
            sendEventParams = new object[1];
        }

        public static bool SendEvent(object view, Event evt)
        {

            sendEventParams[0] = evt;

            return (bool)sendEvent.Invoke(view, sendEventParams);
        }

        public static void RepaintCurrent()
        {

            repaintInfo.Invoke(Current, null);
        }

        public static void RepaintCurrentImmediately()
        {

            repaintImmediatelyInfo.Invoke(Current, null);
        }
    }
}
