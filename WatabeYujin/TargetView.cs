using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[CustomEditor(typeof(TargetObject))]
public class TargetView : Editor
{
    private Text m_textField;

    public override void OnInspectorGUI()
    {
        TargetObject targetObject = target as TargetObject;

        targetObject.breakEffect = EditorGUILayout.ObjectField("breakEffect",targetObject.breakEffect, typeof(GameObject), true)as GameObject;
        targetObject.targetType = (TargetObject.TargetType)EditorGUILayout.EnumPopup("TargetType", targetObject.targetType);
        targetObject.targetMoveType = (TargetObject.TargetMoveType)EditorGUILayout.EnumPopup("TargetMoveType", targetObject.targetMoveType);
        targetObject.renderer = EditorGUILayout.ObjectField("Renderer", targetObject.renderer, typeof(Renderer), true) as Renderer;

        if (targetObject.targetType == TargetObject.TargetType.Composite)
        {
            targetObject.bossCommand1Player = EditorGUILayout.IntField("1P向けボス用コマンド入力", targetObject.bossCommand1Player);
            targetObject.bossCommand2Player = EditorGUILayout.IntField("2P向けボス用コマンド入力", targetObject.bossCommand1Player);
        }
    }
}
