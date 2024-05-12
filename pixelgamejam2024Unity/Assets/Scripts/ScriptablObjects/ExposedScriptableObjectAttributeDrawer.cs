using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// This class allows Scriptable Objects to be seen and edited from the class they are attached to in the inspector. Simply add [ExposedScriptableObject] above the line calling the SO. Example:
/// [SerializeField] [ExposedScriptableObject]
/// private EntitySO entity;
/// </summary>
/// 
[CustomPropertyDrawer(typeof(ExposedScriptableObjectAttribute))]
public class ExposedScriptableObjectAttributeDrawer : PropertyDrawer
{
    private Editor _editor;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //Draw label
        EditorGUI.PropertyField(position, property, label, true);

        //Draw foldout arrow
        if (property.objectReferenceValue != null)
        {
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, GUIContent.none);
        }

        //Draw foldout properties
        if (!property.isExpanded) return;
        //Indent child fields
        EditorGUI.indentLevel++;
            
        //Draw object properties
        if(!_editor) Editor.CreateCachedEditor(property.objectReferenceValue, null, ref _editor);
        _editor.OnInspectorGUI();

        //Set indent back
        EditorGUI.indentLevel--;
    }
}

public class ExposedScriptableObjectAttribute : PropertyAttribute
{
    
}
