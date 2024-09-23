/**
 * https://forum.unity.com/threads/extend-editorgui.465968/
 */

using System;
using System.Reflection;
using UnityEngine;

namespace TLab.UI.SDF.Editor
{
    public static class GUIView
    {
        private static Type m_guiViewType;
        private static MethodInfo m_sendEvent;
        private static MethodInfo m_repaintInfo;
        private static PropertyInfo m_currentInfo;
        private static MethodInfo m_repaintImmediatelyInfo;
        private static object[] m_sendEventParams;

        public static object current => m_currentInfo.GetValue(null, null);

        static GUIView()
        {
            m_guiViewType = Type.GetType("UnityEditor.GUIView, UnityEditor");
            m_sendEvent = m_guiViewType.GetMethod("SendEvent", BindingFlags.NonPublic | BindingFlags.Instance);
            m_currentInfo = m_guiViewType.GetProperty("current", BindingFlags.Public | BindingFlags.Static);
            m_repaintInfo = m_guiViewType.GetMethod("Repaint", BindingFlags.Public | BindingFlags.Instance);
            m_repaintImmediatelyInfo = m_guiViewType.GetMethod("RepaintImmediately", BindingFlags.Public | BindingFlags.Instance);
            m_sendEventParams = new object[1];
        }

        public static bool SendEvent(object view, Event evt)
        {
            m_sendEventParams[0] = evt;

            return (bool)m_sendEvent.Invoke(view, m_sendEventParams);
        }

        public static void RepaintCurrent() => m_repaintInfo.Invoke(current, null);

        public static void RepaintCurrentImmediately() => m_repaintImmediatelyInfo.Invoke(current, null);
    }
}
