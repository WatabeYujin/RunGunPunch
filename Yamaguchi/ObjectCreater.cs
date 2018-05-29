using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

/// <summary>
/// 障害物を生成するエディタ
/// </summary>
public class ObjectCreater : EditorWindow {

    private GameObject parent;                                      // 障害物の親のオブジェクト
    private List<GameObject> prefabs = new List<GameObject>();      // 生成するプレファブ
    private GameObject tObj = null;                                 // listに入れておく変数
    private int numZ = 1;                                           // Z軸に生成するオブジェクトの数
    private float intervalZ = 1;                                    // Z軸に生成するオブジェクトの間隔

    private int row = 3;                                            // 障害物の行
    private int column = 2;                                         // 障害物の列
    private Vector3 pos;                                            // 障害物の行と列の情報を格納

    private string outputFileName;                                  // 出力するファイルの名前
    private string dirPath = "Assets/Obstacle/";                    // 出力するディレクトリのパス
    private const int range = 3;                                    // 障害物の全体の範囲

    string s = "dsaffffffffffff\ndfasfafsfasfaf\n";
/// ////////////////////////////////////////////////////////////////////////////////////////////////////////


    [MenuItem("Tool/ObjectCreater")]
    static void Init()
    {
        EditorWindow.GetWindow<ObjectCreater>(true, "ObjectCreater"); // ウィンドウを生成
    }

    void OnEnable()
    {
        if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];    // エディタを開いた際に選択していたオブジェクトをparentに設定する
    }

    private void OnGUI()
    {
        EditorGUILayout.Space();
        
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject),true) as GameObject;      

        GUILayout.Space(20.0f);

        numZ = int.Parse(EditorGUILayout.TextField("num", numZ.ToString()));                                // 生成するオブジェクトの数を設定

        intervalZ = int.Parse(EditorGUILayout.TextField("interval", intervalZ.ToString()));                 // オブジェクトを生成する間隔

        GUILayout.Space(20.0f);

        for (int i = 0; i < numZ; i++)
        {
            prefabs.Add(tObj);
            prefabs[i] = EditorGUILayout.ObjectField("Prefab" + i, prefabs[i], typeof(GameObject), true) as GameObject;
            EditorGUILayout.Space();
        }
        
        

        GenerationPosition();       //プレファブを生成する位置を指定

        EditorGUILayout.Space();

        if (GUILayout.Button("Create")) Create();       //プレファブを生成

        GUILayout.Space(100);

        using(new GUILayout.VerticalScope(EditorStyles.helpBox))
        {
            using(new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                outputFileName = EditorGUILayout.TextField("PrefabName ",outputFileName);     //保存するプレファブの名前の設定
            }

            using(new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save")) {
                    
                    SavePrefab();       //障害物を保存

                }

            }
        }

        EditorGUILayout.TextArea(s);
    }

/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

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
                    pos.x = j * range;      //障害物のX方向の間隔の設定

                    pos.y = i * range;      //障害物のY方向の間隔の設定

                    //if (j == 2) pos.y = i * 3 + 1;
                  
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

        int m_count = 0;                                                                              // 生成したオブジェクトに付ける番号

        pos.z = -(numZ - 1) * intervalZ / 2;                                                        // 最初に生成するオブジェクトのポジションZ
        for (int i = 0; i < numZ; i++)
        {
            if (prefabs[i])
            {
                GameObject m_obj = Instantiate(prefabs[i], pos, Quaternion.identity) as GameObject;       // オブジェクト生成

                m_obj.name = prefabs[i].name + m_count++;                                                   // 生成したオブジェクトの名前に番号を付ける

                if (parent) m_obj.transform.parent = parent.transform;                                    // parentがあればparentの子オブジェクトにする

                Undo.RegisterCreatedObjectUndo(m_obj, "Object Creater");                                  // オブジェクトを生成したものをUndo履歴に入れる
            }
            

            pos.z += intervalZ;                                                                     // ポジションZをintervalZの数値だけ間隔をあける
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
            
            Directory.CreateDirectory(dirPath);                            //prefab保存用のフォルダがなければ作成する
        }

        
        string m_prefabPath = dirPath + outputFileName + ".prefab";          //prefabの保存ファイルパス
        
        
        UnityEditor.PrefabUtility.CreatePrefab(m_prefabPath, parent);         //指定のファイルにprefabファイルを作成

        UnityEditor.AssetDatabase.SaveAssets();                             //prefabの保存
    }
}

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
