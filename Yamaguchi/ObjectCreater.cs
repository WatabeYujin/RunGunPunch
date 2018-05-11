using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class ObjectCreater : EditorWindow {

    private GameObject parent; //障害物の親のオブジェクト
    private GameObject prefab; //生成するプレファブ
    //private int numX = 1; //X軸に生成するオブジェクトの数
    //private int numY = 1; //Y軸に生成するオブジェクトの数
    private int numZ = 1; //Z軸に生成するオブジェクトの数
    //private float intervalX = 1; //X軸に生成するオブジェクトの間隔
    //private float intervalY = 1; //Y軸に生成するオブジェクトの間隔
    private float intervalZ = 1; //Z軸に生成するオブジェクトの間隔

    private int row = 3; //障害物の行
    private int column = 2; //障害物の列
    private Vector3 pos = new Vector3(1,1,0); //障害物の行と列の情報を格納

    private string outputFileName; //出力するファイルの名前
    private string dirPath = "Assets/Obstacle/"; //出力するディレクトリのパス

    [MenuItem("Tool/ObjectCreater")]
    static void Init()
    {
        EditorWindow.GetWindow<ObjectCreater>(true, "ObjectCreater");
    }

    void OnEnable()
    {
        if (Selection.gameObjects.Length > 0) parent = Selection.gameObjects[0];
    }

    void OnSelectionChange()
    {
        if (Selection.gameObjects.Length > 0) prefab = Selection.gameObjects[0];
        Repaint();
    }

    private void OnGUI()
    {
        parent = EditorGUILayout.ObjectField("Parent", parent, typeof(GameObject),true) as GameObject;
        prefab = EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), true) as GameObject;

        /*GUILayout.Label("X : ", EditorStyles.boldLabel);
        numX = int.Parse(EditorGUILayout.TextField("num", numX.ToString()));
        intervalX = int.Parse(EditorGUILayout.TextField("interval", intervalX.ToString()));

        GUILayout.Label("Y : ", EditorStyles.boldLabel);
        numY = int.Parse(EditorGUILayout.TextField("num", numY.ToString()));
        intervalY = int.Parse(EditorGUILayout.TextField("interval", intervalY.ToString()));*/

        GUILayout.Label("Z : ", EditorStyles.boldLabel, GUILayout.Width(110));
        numZ = int.Parse(EditorGUILayout.TextField("num", numZ.ToString()));
        intervalZ = int.Parse(EditorGUILayout.TextField("interval", intervalZ.ToString()));

        EditorGUILayout.Space();
        GenerationPosition();

        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create")) Create();

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
                if (GUILayout.Button("Save")) SavePrefab();
            }
        }
    }

    //障害物を生成する位置の設定
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
                    pos.x = j * 3;
                    pos.y = i * 3;
                    pos.z = 0;
                    //if (j == 2) pos.y = i * 3 + 1;
                    Debug.Log("posX " + pos.x + "posY " + pos.y + "posZ" + pos.z);
                }
            }
            GUILayout.EndHorizontal();
        }
        EditorGUILayout.EndVertical();

        /*using(new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            for(int i = column; i > 0; i--)
            {
                using (new EditorGUILayout.HorizontalScope())
                {
                    for (int j = row; j > 0; j--)
                    {
                        if (GUILayout.Button(""))
                        {
                            pos.x = j*3;
                            pos.y = i*3;
                            //if (j == 2) pos.y = i * 3 + 1;
                        }
                    }
                }
            }
            
        }*/
    }

    //オブジェクトの生成
    private void Create()
    {
        if (prefab == null) return;

        int count = 0;

        /*pos.x = -(numX - 1) * intervalX / 2;
        for (int x = 0; x < numX; x++)
        {
            pos.y = -(numY - 1) * intervalY / 2;
            for (int y = 0; y < numY; y++)
            {
                pos.z = -(numZ - 1) * intervalZ / 2;
                for (int z = 0; z < numZ; z++)
                {
                    GameObject obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
                    obj.name = prefab.name + count++;
                    if (parent) obj.transform.parent = parent.transform;
                    Undo.RegisterCreatedObjectUndo(obj, "Object Creater");

                    pos.z += intervalZ;
                }
                pos.y += intervalY;
            }
            pos.x += intervalX;
        }*/
        pos.z = -(numZ - 1) * intervalZ / 2;
        for (int z = 0; z < numZ; z++)
        {
            GameObject obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
            obj.name = prefab.name + count++;
            if (parent) obj.transform.parent = parent.transform;
            Undo.RegisterCreatedObjectUndo(obj, "Object Creater");

            pos.z += intervalZ;
            //pos.y -= z * 0.5f;
        } 
        
    }

    //プレファブをセーブ
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
