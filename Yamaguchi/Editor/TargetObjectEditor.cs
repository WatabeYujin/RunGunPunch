using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TargetObject))]
public class TargetObjectEditor : Editor {

    /// <summary>
    /// inspectorのGUIを更新

    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();  //元のinspector部分を表示

        TargetObject targetObject = target as TargetObject; //targetを変換して対象を取得

        if (GUILayout.Button("TargetTypeChange")) targetObject.TargetTypeChange();
    }
}
