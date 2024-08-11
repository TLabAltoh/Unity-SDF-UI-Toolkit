using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TLab.UI.SDF
{
    public static class ControlPointUtils
    {
        internal const string MAX_CONTROL_NUM_PREFIX = "MAX_CONTROL_NUM_";

        private const string THIS_NAME = "[ControlPointUtils] ";

        public readonly static Dictionary<int, string> shaderKeywords = new()
        {
            { 1, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "1" },
            { 3, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "3" },
            { 5, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "5" },
            { 10, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "10" },
            { 15, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "15" },
            { 20, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "20" },
            { 30, SDFUI.SHADER_KEYWORD_PREFIX + MAX_CONTROL_NUM_PREFIX + "30" },
        };

        public static bool TryGetShaderKeywordForEnable(int num, out string shaderKeyword)
        {
            shaderKeyword = null;

            for (int i = 0; i < shaderKeywords.Count - 1; i++)
            {
                if (shaderKeywords.ElementAt(i).Key <= i && shaderKeywords.ElementAt(i + 1).Key >= i)
                {
                    shaderKeyword = shaderKeywords[i];
                    return true;
                }
            }

            Debug.LogError(THIS_NAME + $"An invalid num has been given (out of range): {num}");

            return false;
        }

        public static void GetShaderKeywordForDisable(int num, out string[] shaderKeywords)
        {
            var list = new List<string>();
            for (int i = 0; i < ControlPointUtils.shaderKeywords.Count; i++)
            {
                var elem = ControlPointUtils.shaderKeywords.ElementAt(i);
                if (elem.Key < i)
                    list.Add(elem.Value);
            }
            shaderKeywords = list.ToArray();
        }
    }
}
