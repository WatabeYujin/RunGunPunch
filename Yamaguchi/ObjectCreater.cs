using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 障害物を生成するエディタ
/// </summary>
public class ObjectCreater : EditorWindow {

    private GameObject parent;                                      //障害物の親のオブジェクト
    private List<GameObject> prefabs = new List<GameObject>();      //生成するプレファブ
    private GameObject tObj = null;                                 //listに入れておく変数
    private int numZ = 1;                                           //Z軸に生成するオブジェクトの数
    private float intervalZ = 1;                                    //Z軸に生成するオブジェクトの間隔

    private int row = 3;                                            //障害物の行
    private int column = 2;                                         //障害物の列
    private Vector3 pos;                                            //障害物の行と列の情報を格納

    private string outputFileName;                                  //出力するファイルの名前
    private string dirPath = "Assets/Obstacle/";                    //出力するディレクトリのパス
    private const int range = 3;                                    //障害物の全体の範囲
    

    [MenuItem("Tool/ObjectCreater")]
    static void Init()
    {
        EditorWindow.GetWindow<ObjectCreater>(true, "ObjectCreater"); //ウィンドウを生成
    }

    void OnEnable()
    {
        if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];    
    }

    void OnSelectionChange()
    {
        if (Selection.gameObjects.Length > 0) prefabs[0] = Selection.gameObjects[0];
        Repaint();
    }

    private void OnGUI()
    {
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject),true) as GameObject;

        GUILayout.Label("Z : ", EditorStyles.boldLabel, GUILayout.Width(110));
        numZ = int.Parse(EditorGUILayout.TextField("num", numZ.ToString()));
        intervalZ = int.Parse(EditorGUILayout.TextField("interval", intervalZ.ToString()));

        

        for (int i = 0; i < numZ; i++)
        {
            prefabs.Add(tObj);
            prefabs[i] = EditorGUILayout.ObjectField("Prefab" + i, prefabs[i], typeof(GameObject), true) as GameObject;          
        }
        
        EditorGUILayout.Space();

        GenerationPosition();       //プレファブを生成する位置を指定

        EditorGUILayout.Space();

        if (GUILayout.Button("Create")) Create();       //プレファブを生成

        GUILayout.Space(100);

        using(new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("PrefabName ");
                outputFileName = EditorGUILayout.TextField(outputFileName);
            }

            using(new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save")) {
                    
                    SavePrefab();       //障害物を保存

                }
                
                
            }
        }
    }

    /// <summary>
    /// 障害物を生成する位置の設定
    /// </summary>
    private void GenerationPosition()
    {
        GUILayout.Label("Generation position : ", EditorStyles.boldLabel);

        EditorGUILayout.BeginVertical(EditorStyles.helpBox);
        for (int i = column; i > 0; i--)
        {
            GUILayout.BeginHorizontal();
            for (int j = row; j > 0; j--)
            {
                if (GUILayout.Button(""))
                {
                    pos.x = j * range;
                    pos.y = i * range;
                    pos.z = 0;
                    //if (j == 2) pos.y = i * 3 + 1;
                    Debug.Log("posX " + pos.x + "posY " + pos.y + "posZ" + pos.z);
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

    }

    /// <summary>
    /// オブジェクトの生成
    /// </summary>
    private void Create()
    {
        if (prefabs[0] == null) return;

        int count = 0;

        pos.z = -(numZ - 1) * intervalZ / 2;
        for (int i = 0; i < numZ; i++)
        {
            GameObject obj = Instantiate(prefabs[i], pos, Quaternion.identity) as GameObject;
            obj.name = prefabs[i].name + count++;
            if (parent) obj.transform.parent = parent.transform;
            Undo.RegisterCreatedObjectUndo(obj, "Object Creater");

            pos.z += intervalZ;
            //pos.y -= z * 0.5f;
        } 
        
    }

    /// <summary>
    /// プレファブをセーブ
    /// </summary>
    private void SavePrefab()
    {
        if (!Directory.Exists(dirPath))
        {
            //prefab保存用のフォルダがなければ作成する
            Directory.CreateDirectory(dirPath);
        }

        //prefabの保存ファイルパス
        string prefabPath = dirPath + outputFileName + ".prefab";
        if (!File.Exists(prefabPath))
        {
            //prefabファイルがなければ作成する
            File.Create(prefabPath);
        }

        //prefabの保存
        UnityEditor.PrefabUtility.CreatePrefab(prefabPath, parent);
        UnityEditor.AssetDatabase.SaveAssets();
    }
}
