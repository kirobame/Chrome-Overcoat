using UnityEditor;
using UnityEngine;

namespace Flux.Editor
{
    public class Vector2Drawer : PropertyDrawer
    {
        public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(rect, label, property);
            var labelWidth = EditorGUIUtility.labelWidth;

            var spacing = 4.0f;
            var labelRect = new Rect(rect.position, new Vector2(EditorGUIUtility.labelWidth, rect.height));
            rect.width -= labelRect.width;

            EditorGUI.LabelField(labelRect, label);

            var halfWidth = rect.width / 2.0f - spacing;
            EditorGUIUtility.labelWidth = halfWidth * 0.15f;
            
            var xRect = new Rect(new Vector2(labelRect.xMax + spacing, rect.yMin), new Vector2(halfWidth, rect.height));

            property.NextVisible(true);
            xRect = EditorGUI.PrefixLabel(xRect, new GUIContent("X"));
            property.floatValue = EditorGUI.FloatField(xRect, GUIContent.none, property.floatValue);
            
            var yRect = new Rect(new Vector2(xRect.xMax + spacing, rect.yMin), new Vector2(halfWidth, rect.height));

            property.NextVisible(false);
            yRect = EditorGUI.PrefixLabel(yRect, new GUIContent("Y"));
            property.floatValue = EditorGUI.FloatField(yRect, GUIContent.none, property.floatValue);

            EditorGUIUtility.labelWidth = labelWidth;
            
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUIUtility.singleLineHeight;
    }
}