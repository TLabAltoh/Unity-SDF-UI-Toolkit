using UnityEditor;
using UnityEngine;
using static TLab.UI.SDF.SDFOp;

namespace TLab.UI.SDF.Editor {

    [CustomPropertyDrawer(typeof(SdfElement))]
    public class SdfElementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty shapeProp = property.FindPropertyRelative(nameof(SdfElement.shape));
            SerializedProperty parametersProp = property.FindPropertyRelative(nameof(SdfElement.parameters));
            SerializedProperty positionProp = property.FindPropertyRelative(nameof(SdfElement.position));
            SerializedProperty scaleProp = property.FindPropertyRelative(nameof(SdfElement.scale));
            SerializedProperty rotationProp = property.FindPropertyRelative(nameof(SdfElement.rotation));
            SerializedProperty onionProp = property.FindPropertyRelative(nameof(SdfElement.onion));
            SerializedProperty boolOpProp = property.FindPropertyRelative(nameof(SdfElement.boolOp));
            SerializedProperty boolSmoothProp = property.FindPropertyRelative(nameof(SdfElement.boolSmooth));
            SerializedProperty colorProp = property.FindPropertyRelative(nameof(SdfElement.color));

            // Get the element index if this property is part of an array (-1 if it's a standalone field)
            int elementIndex = GetPropertyElementIndex(property);
            bool isFirstElement = elementIndex == 0;

            // Generate header text
            string headerText = label.text;
            if (shapeProp != null)
            {
                var shape = (SdfShape)shapeProp.enumValueIndex;
                if (isFirstElement || boolOpProp == null)
                {
                    headerText += $" [{shape}] (Base Shape)";
                }
                else
                {
                    var op = (SdfBoolOp)boolOpProp.enumValueIndex;
                    headerText += $" [{shape}] (Op: {op})";
                }
            }

            Rect singleLineRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            property.isExpanded = EditorGUI.Foldout(singleLineRect, property.isExpanded, new GUIContent(headerText), true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                Rect currentRect = singleLineRect;
                float lineOffset = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

                // Shape Settings
                currentRect.y += lineOffset;
                EditorGUI.PropertyField(currentRect, shapeProp);

                SdfShape currentShape = (SdfShape)shapeProp.enumValueIndex;
                DrawParametersField(ref currentRect, lineOffset, parametersProp, positionProp, currentShape);

                // Transform Settings
                currentRect.y += lineOffset;
                DrawCenterPositionField(currentRect, positionProp);
                currentRect.y += lineOffset;
                EditorGUI.PropertyField(currentRect, scaleProp);
                scaleProp.floatValue = Mathf.Max(0f, scaleProp.floatValue); // Min = 0
                currentRect.y += lineOffset;
                EditorGUI.PropertyField(currentRect, rotationProp);
                currentRect.y += lineOffset;
                EditorGUI.PropertyField(currentRect, onionProp);
                onionProp.floatValue = Mathf.Max(0f, onionProp.floatValue); // Min = 0

                // Boolean Operations (Only drawn if Index > 0)
                if (!isFirstElement)
                {
                    currentRect.y += lineOffset;
                    EditorGUI.PropertyField(currentRect, boolOpProp);

                    currentRect.y += lineOffset;
                    EditorGUI.PropertyField(currentRect, boolSmoothProp);
                    boolSmoothProp.floatValue = Mathf.Max(0f, boolSmoothProp.floatValue); // Min = 0
                }

                // Color Settings
                currentRect.y += lineOffset;
                EditorGUI.PropertyField(currentRect, colorProp);

                EditorGUI.indentLevel--;
            }


            EditorGUI.EndProperty();
        }

        /// <summary>
        /// Draws only the X and Y components of the position field as "Center (XY)" in the inspector.
        /// </summary>
        private void DrawCenterPositionField(Rect rect, SerializedProperty posProp)
        {
            Vector4 posVal = posProp.vector4Value;
            Vector2 center = new Vector2(posVal.x, posVal.y);

            // Draw as a Vector2Field in the inspector
            center = EditorGUI.Vector2Field(rect, "Center (XY)", center);

            // Write the edited values back to the X and Y components of the original Vector4
            posVal.x = center.x;
            posVal.y = center.y;
            posProp.vector4Value = posVal;
        }

        private void DrawParametersField(ref Rect rect, float lineOffset, SerializedProperty paramsProp, SerializedProperty posProp, SdfShape shape)
        {
            Vector4 val = paramsProp.vector4Value;
            Vector4 posVal = posProp.vector4Value;

            switch (shape)
            {
                case SdfShape.Circle:
                    {
                        /***
                        * x: Radius
                        * y: None
                        * z: None
                        * w: None
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Radius", val.x)); // Min = 0
                    }
                    break;

                case SdfShape.Arc:
                    {
                        /***
                        * x: Ratio
                        * y: Radius
                        * z: Width
                        * w: CornerRounding
                        */
                        rect.y += lineOffset;
                        val.x = EditorGUI.Slider(rect, "Ratio", val.x, -1f, 1f);

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Radius", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = Mathf.Max(0f, EditorGUI.FloatField(rect, "Width", val.z)); // Min = 0

                        rect.y += lineOffset;
                        float inputRounding = Mathf.Max(0f, EditorGUI.FloatField(rect, "Corner Rounding", val.w)); // Min = 0

                        // Clamp the upper limit to half of the width
                        float maxArcRounding = val.z * 0.5f;
                        val.w = Mathf.Clamp(inputRounding, 0f, maxArcRounding);
                    }
                    break;


                case SdfShape.Triangle:
                    {
                        /***
                        * x: Base of a triangle
                        * y: Height of a triangle
                        * z: Roundness
                        * w: AnchorY
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Base", val.x)); // Min = 0

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = Mathf.Max(0f, EditorGUI.FloatField(rect, "Roundness", val.z)); // Min = 0

                        rect.y += lineOffset;
                        val.w = EditorGUI.FloatField(rect, "Anchor Y", val.w);
                    }
                    break;

                case SdfShape.Quad:
                    {
                        /***
                        * x: Top right corner radius
                        * y: Bottom right corner radius
                        * z: Top left corner radius
                        * w: Bottom left corner radius
                        * 
                        * position.z: Width
                        * position.w: Height
                        */
                        rect.y += lineOffset;
                        posVal.z = Mathf.Max(0f, EditorGUI.FloatField(rect, "Width", posVal.z)); // Min = 0

                        rect.y += lineOffset;
                        posVal.w = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", posVal.w)); // Min = 0

                        rect.y += lineOffset;
                        val = EditorGUI.Vector4Field(rect, "Corner Rounding (TR / BR / TL / BL)", val);

                        // Calculate the maximum allowed radius (half of the shorter side between Width and Height)
                        float maxRadius = Mathf.Min(posVal.z, posVal.w) * 0.5f;

                        // Clamp between the lower limit of 0f and the upper limit of maxRadius
                        val.x = Mathf.Clamp(val.x, 0f, maxRadius);
                        val.y = Mathf.Clamp(val.y, 0f, maxRadius);
                        val.z = Mathf.Clamp(val.z, 0f, maxRadius);
                        val.w = Mathf.Clamp(val.w, 0f, maxRadius);
                    }
                    break;

                case SdfShape.Parallelogram:
                    {
                        /***
                        * x: Width
                        * y: Height
                        * z: Slide
                        * w: Roundness
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Width", val.x)); // Min = 0

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = EditorGUI.FloatField(rect, "Slide", val.z);

                        rect.y += lineOffset;
                        val.w = Mathf.Max(0f, EditorGUI.FloatField(rect, "Roundness", val.w)); // Min = 0
                    }
                    break;

                case SdfShape.Vesica:
                    {
                        /***
                        * x: Width
                        * y: Height
                        * z: Roundness
                        * w: None
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Width", val.x)); // Min = 0

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = Mathf.Max(0f, EditorGUI.FloatField(rect, "Roundness", val.z)); // Min = 0
                    }
                    break;

                case SdfShape.Moon:
                    {
                        /***
                        * x: Radius A
                        * y: Radius B
                        * z: Slide
                        * w: Roundness
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Radius", val.x)); // Min = 0

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Crop Radius", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = EditorGUI.FloatField(rect, "Slide", val.z);

                        rect.y += lineOffset;
                        val.w = Mathf.Max(0f, EditorGUI.FloatField(rect, "Roundness", val.w)); // Min = 0
                    }
                    break;

                case SdfShape.Egg:
                    {
                        /***
                        * x: Bluge
                        * y: Height
                        * z: Radius A
                        * w: Radius B
                        */
                        rect.y += lineOffset;
                        val.x = EditorGUI.Slider(rect, "Bluge", val.x, 0f, 1f);

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", val.y)); // Min = 0

                        rect.y += lineOffset;
                        val.z = Mathf.Max(0f, EditorGUI.FloatField(rect, "Radius A", val.z)); // Min = 0

                        rect.y += lineOffset;
                        val.w = Mathf.Max(0f, EditorGUI.FloatField(rect, "Radius B", val.w)); // Min = 0
                    }
                    break;

                case SdfShape.Ellipse:
                    {
                        /***
                        * x: Width
                        * y: Height
                        * z: None
                        * w: None
                        */
                        rect.y += lineOffset;
                        val.x = Mathf.Max(0f, EditorGUI.FloatField(rect, "Width", val.x)); // Min = 0

                        rect.y += lineOffset;
                        val.y = Mathf.Max(0f, EditorGUI.FloatField(rect, "Height", val.y)); // Min = 0
                    }
                    break;
            }

            paramsProp.vector4Value = val;
            posProp.vector4Value = posVal;
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // Only 1 line height is needed if the property is collapsed
            if (!property.isExpanded)
            {
                return EditorGUIUtility.singleLineHeight;
            }

            // Base lines excluding unique fields separated from parameters / position
            // Foldout + shape + position(Center) + scale + rotation + onion + color = Total 7 lines
            float lineCount = 7;

            // Add required lines for each shape type
            SerializedProperty shapeProp = property.FindPropertyRelative(nameof(SdfElement.shape));
            if (shapeProp != null)
            {
                SdfShape currentShape = (SdfShape)shapeProp.enumValueIndex;
                switch (currentShape)
                {
                    case SdfShape.Circle:
                        lineCount += 1; // Radius
                        break;

                    case SdfShape.Triangle:
                        lineCount += 4; // Base, Height, Roundness, AnchorY
                        break;

                    case SdfShape.Quad:
                        lineCount += 3; // Width, Height, Corner Rounding (Vector4 counts as 1 line)
                        break;

                    case SdfShape.Parallelogram:
                        lineCount += 4; // Width, Height, Slide, Roundness
                        break;

                    case SdfShape.Arc:
                        lineCount += 4; // Theta, Radius, Width, Corner Rounding
                        break;

                    case SdfShape.Vesica:
                        lineCount += 3; // Width, Height, Roundness
                        break;

                    case SdfShape.Moon:
                        lineCount += 4; // Width, Height, Slide, Roundness
                        break;

                    case SdfShape.Egg:
                        lineCount += 4; // Bluge, Height, Radius A, Radius B
                        break;

                    case SdfShape.Ellipse:
                        lineCount += 2; // Width, Height
                        break;
                }
            }

            // Add 2 lines for boolean operations (boolOp, boolSmooth) if this is not the first element (Index != 0)
            int elementIndex = GetPropertyElementIndex(property);
            if (elementIndex != 0)
            {
                lineCount += 2;
            }

            // Calculate final pixel height considering standard vertical spacing between lines
            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return (lineHeight * lineCount) + (spacing * (lineCount - 1));
        }

        /// <summary>
        /// Utility to extract the element index from a PropertyPath.
        /// </summary>
        private int GetPropertyElementIndex(SerializedProperty property)
        {
            string path = property.propertyPath;

            // If it's an array element, leverage the fact that the path ends with ".data[index]"
            if (path.EndsWith("]"))
            {
                int startIndex = path.LastIndexOf('[') + 1;
                int length = path.LastIndexOf(']') - startIndex;
                if (int.TryParse(path.Substring(startIndex, length), out int index))
                {
                    return index;
                }
            }
            return -1; // Returns -1 if it's a normal standalone field rather than an array element
        }
    }
}
