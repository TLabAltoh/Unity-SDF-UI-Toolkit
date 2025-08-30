using UnityEngine;
using UnityEditor;

namespace TLab.UI.SDF.Editor
{
    public class CaptureUtil
    {
        [MenuItem("TLab/UI/SDF/Capture GameView")]
        public static void Grab()
        {
            ScreenCapture.CaptureScreenshot("Assets/Screenshot" +
                GetCh() + GetCh() + GetCh() + "_" +
                Screen.width + "x" + Screen.height + ".png", 1);
        }

        public static char GetCh()
        {
            return (char)Random.Range('A', 'Z');
        }
    }

}
