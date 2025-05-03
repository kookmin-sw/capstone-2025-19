using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(SerializableArray<,>))]
public class SerializableArrayDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var keysProperty = property.FindPropertyRelative("keys");
        var valuesProperty = property.FindPropertyRelative ("values");
        var foldoutProperty = property.FindPropertyRelative("foldout");

        //foldout 상태에 따라 표시
        foldoutProperty.boolValue = EditorGUI.Foldout(new Rect(position.x, position.y, position.width,
            EditorGUIUtility.singleLineHeight), foldoutProperty.boolValue, label);

        if (foldoutProperty.boolValue)
        {
            EditorGUI.indentLevel++;

            for (int i = 0; i < keysProperty.arraySize; i++)
            {
                var keyProperty = keysProperty.GetArrayElementAtIndex(i);
                var valueProperty = valuesProperty.GetArrayElementAtIndex(i);

                float lineHeight = EditorGUIUtility.singleLineHeight;
                float spacing = EditorGUIUtility.standardVerticalSpacing;

                Rect keyRect = new Rect(position.x, position.y + (lineHeight + spacing) * (i + 1), position.width / 2 - 5, lineHeight);
                Rect valueRect = new Rect(position.x + position.width / 2 + 5, position.y + (lineHeight + spacing) * (i + 1), position.width / 2 - 5, lineHeight);

                EditorGUI.PropertyField(keyRect, keyProperty, GUIContent.none);
                EditorGUI.PropertyField(valueRect, valueProperty, GUIContent.none);

                // Remove button
                Rect removeButtonRect = new Rect(position.x + position.width - 20, position.y + (lineHeight + spacing) * (i + 1), 20, lineHeight);
                if (GUI.Button(removeButtonRect, "-"))
                {
                    keysProperty.DeleteArrayElementAtIndex(i);
                    valuesProperty.DeleteArrayElementAtIndex(i);
                }
            }

            if (GUI.Button(new Rect(position.x, position.y + (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) 
                * (keysProperty.arraySize + 1), position.width, EditorGUIUtility.singleLineHeight), "Add Entry"))
            {
                keysProperty.arraySize++;
                valuesProperty.arraySize++;

                var newKeyProperty = keysProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);
                var newValueProperty = valuesProperty.GetArrayElementAtIndex(keysProperty.arraySize - 1);

                SetDefaultKeyAndValue(newKeyProperty, newValueProperty);
            }

            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }

    private void SetDefaultKeyAndValue(SerializedProperty keyProperty, SerializedProperty valueProperty)
    {
        // Set default key and value
        if (keyProperty.propertyType == SerializedPropertyType.String)
        {
            keyProperty.stringValue = "New Key";
        }
        else if (keyProperty.propertyType == SerializedPropertyType.Integer)
        {
            keyProperty.intValue = 0;
        }
        else if (keyProperty.propertyType == SerializedPropertyType.Float)
        {
            keyProperty.floatValue = 0f;
        }
        else if (keyProperty.propertyType == SerializedPropertyType.ObjectReference)
        {
            if (keyProperty.objectReferenceValue == null)
            {
                keyProperty.objectReferenceValue = ScriptableObject.CreateInstance<ItemData>();
            }
        }

        // Set default value
        if (valueProperty.propertyType == SerializedPropertyType.String)
        {
            valueProperty.stringValue = "New Value";
        }
        else if (valueProperty.propertyType == SerializedPropertyType.Integer)
        {
            valueProperty.intValue = 0;
        }
        else if (valueProperty.propertyType == SerializedPropertyType.Float)
        {
            valueProperty.floatValue = 0f;
        }
        else if (valueProperty.propertyType == SerializedPropertyType.ObjectReference)
        {
            valueProperty.objectReferenceValue = null;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var keysProperty = property.FindPropertyRelative("keys");
        var foldoutProperty = property.FindPropertyRelative("foldout");

        if (foldoutProperty != null && foldoutProperty.boolValue)
        {
            return (keysProperty.arraySize + 2) * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

}

