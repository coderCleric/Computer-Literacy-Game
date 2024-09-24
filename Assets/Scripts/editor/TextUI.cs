using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Text))]
public class TextUI : Editor
{
    private static int fontChange = 0;

    public override void OnInspectorGUI()
    {
        Text text = (Text)target;
        base.OnInspectorGUI();

        fontChange = EditorGUILayout.IntField("Font size change: ", fontChange);
        if (GUILayout.Button("Resize text"))
        {
            text.fontSize += fontChange;
        }
        if (GUILayout.Button("Resize all"))
        {
            foreach(Text txt in Component.FindObjectsOfType(typeof(Text), true))
                txt.fontSize += fontChange;
        }
    }
}
