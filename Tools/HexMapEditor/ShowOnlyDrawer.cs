using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace HexMapEditor
{
    [CustomPropertyDrawer(typeof(ShowOnlyAttribute))]
    public class ShowOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string valueStr;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    {
                        valueStr = property.intValue.ToString();
                        break;
                    }
                case SerializedPropertyType.Boolean:
                    {
                        valueStr = property.boolValue.ToString();
                        break;
                    }
                case SerializedPropertyType.Float:
                    {
                        valueStr = property.floatValue.ToString();
                        break;
                    }
                case SerializedPropertyType.String:
                    {
                        valueStr = property.stringValue;
                        break;
                    }
                default:
                    {
                        valueStr = "(not support)";
                        break;
                    }
            }

            EditorGUI.LabelField(position, label.text, valueStr);

            //base.OnGUI(position, property, label);

        }
    }
}
