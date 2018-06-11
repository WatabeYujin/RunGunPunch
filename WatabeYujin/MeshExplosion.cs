using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class MeshExplosion : MonoBehaviour {
    //オブジェクトをバラバラにして吹き飛ばすスクリプト
    [SerializeField]
    int maxTriangles = 30;

    public void Explode(Transform target,Vector3 center,Vector3 moveVerocity)
    {
        Debug.Log("我が名はめぐみん!");
        Debug.Log("紅魔族随一の魔法の使い手にして、爆裂魔法を操りし者。");
        Debug.Log("我が力、見るがいい！");
        Debug.Log("『エクスプロージョン』！!");

        Vector3 m_targetScale = target.transform.localScale;

        GameObject aaa = new GameObject();
        aaa.transform.position = center;
        aaa.name = "centerrrrrrrrrrr";
        aaa.transform.parent = target;

        Mesh mesh = target.GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] triangles = mesh.triangles;
        Vector2[] uvs = mesh.uv;
        int index = 0;

        // remove collider from original
        target.GetComponent<Collider>().enabled = false;

        // get each face
        int m_objcount = triangles.Length / 3;
        m_objcount = m_objcount / maxTriangles;
        Debug.Log(m_objcount);
        //for (int i = 0; i < triangles.Length; i += 3)
        for (int i = 0; i < triangles.Length; i += 3 * m_objcount)
        {
            // TODO: inherit speed, spin...?
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 s = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((s.x + s.y + s.z) / 3) * 0.3f;
            List<Vector3> m_verticesList = new List<Vector3>();
            for (int j = 0; j < 3 * m_objcount; j++)
            {
                if(vertices[i + j]!=null) m_verticesList.Add(vertices[i + j]);
            }
            List<Vector2> m_uvList = new List<Vector2>();
            for(int j=0;j< 3 * m_objcount; j++)
            {
                m_uvList.Add(uvs[triangles[j]]);
            }
            Rigidbody r = CreateMeshPiece(extrudeSize, target.transform.position, target.GetComponent<Renderer>().material, index, averageNormal, m_verticesList, m_uvList);
            
            Vector3 testt = (vertices[triangles[i]] + vertices[triangles[i + 1]] + vertices[triangles[i + 2]]) / 3;
            
            r.AddForce((testt - aaa.transform.localPosition) * 10+ moveVerocity, ForceMode.VelocityChange);
            r.transform.localScale = m_targetScale;
            index++;
        }
        Debug.Log(index);
        Debug.Log(triangles.Length);
        // destroy original
        Destroy(target.gameObject);
    }

    Rigidbody CreateMeshPiece(float extrudeSize, Vector3 pos, Material mat, int index, Vector3 faceNormal, List<Vector3> verticeList, List<Vector2> uvList)
    {
        GameObject go = new GameObject();
        go.name = "piece_" + index;
        Mesh mesh = go.AddComponent<MeshFilter>().mesh;
        go.AddComponent<MeshRenderer>();
        //go.tag = "Explodable"; // set this only if should be able to explode this piece also
        go.GetComponent<Renderer>().material = mat;
        go.transform.position = pos;

        Vector3[] vertices = new Vector3[3 * (verticeList.Count+1)];
        int[] triangles = new int[3 * (verticeList.Count + 1)];
        Vector2[] uvs = new Vector2[3 * (verticeList.Count + 1)];

        // get centroid

        Vector3 m_verticeCemter = Vector3.zero;
        foreach(Vector3 verticesPos in verticeList)
        {
            m_verticeCemter += verticesPos;
        }
        m_verticeCemter /= verticeList.Count;
        m_verticeCemter = m_verticeCemter + ((-faceNormal) * extrudeSize) / 2;
        verticeList.Add(m_verticeCemter);

        List<int[]> m_CombineArray = CombineSet(verticeList.Count);
        Debug.Log(m_CombineArray);
        for(int i = 0;i<triangles.Length;i+=3)
        {
            triangles[i] = m_CombineArray[i/3][0];
            triangles[i+1] = m_CombineArray[i / 3][1];
            triangles[i+2] = m_CombineArray[i / 3][2];
        }
        for (int i = 0; i < uvs.Length; i ++)
        {
            uvs[i] = uvList[i];
            uvs[i] = uvList[i+1];
            uvs[i] = uvList[i+2];

            /*
                    // orig face
        uvs[0] = uv1;
        uvs[1] = uv2;
        uvs[2] = uv3; // todo
                      // right face
        uvs[3] = uv1;
        uvs[4] = uv2;
        uvs[5] = uv3; // todo

        // left face
        uvs[6] = uv1;
        uvs[7] = uv3;
        uvs[8] = uv3;   // todo
                        // bottom face (mirror?) or custom color? or fixed from uv?
        uvs[9] = uv1;
        uvs[10] = uv2;
        uvs[11] = uv1; // todo
             */
        }
        /*
        // orig face
        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        //  right face
        triangles[3] = 5;
        triangles[4] = 4;
        triangles[5] = 3;
        //  left face
        triangles[6] = 6;
        triangles[7] = 7;
        triangles[8] = 8;
        //  bottom face
        triangles[9] = 11;
        triangles[10] = 10;
        triangles[11] = 9;

        // orig face
        uvs[0] = uv1;
        uvs[1] = uv2;
        uvs[2] = uv3; // todo
                      // right face
        uvs[3] = uv1;
        uvs[4] = uv2;
        uvs[5] = uv3; // todo

        // left face
        uvs[6] = uv1;
        uvs[7] = uv3;
        uvs[8] = uv3;   // todo
                        // bottom face (mirror?) or custom color? or fixed from uv?
        uvs[9] = uv1;
        uvs[10] = uv2;
        uvs[11] = uv1; // todo
        */
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


        MeshCollider mc = go.AddComponent<MeshCollider>();

        mc.sharedMesh = mesh;
        mc.convex = true;
        return go.AddComponent<Rigidbody>();

        //go.AddComponent<MeshFader>();
    }

    private List<int[]> CombineSet(int listLength)
    {
        int s = listLength;
        const int m_combineCount = 3;
        int m = m_combineCount;
        for(int i = 1; i < m_combineCount; i++)
        {
            s *= listLength - i;
            m *= m_combineCount - i;
        }
        s = s/m;
        
        List<int[]> m_returnCombine =new List<int[]>();
        
        m_returnCombine.Add(new int[3] { 0, 1, 2 });
        for (int i = 1; i < s; i++)
        {
            int[] n = new int[m_combineCount];
            int m_digit = m_combineCount - 1;
            for (int j = m_combineCount - 1; j >= 0; j--)
            {
                Debug.Log("j"+j);
                n[j] = m_returnCombine[i - 1][j];
                if(m_digit==j && m_returnCombine[i - 1][j] < listLength) n[j]++;
                else m_digit--;
            }
            Debug.Log(m_returnCombine);
            m_returnCombine.Add(n);
        }
        /*
         for (int j = m_combineCount - 1; j >= 0; j--)
            {
                Debug.Log("j"+j);
                if (m_digit > j) n[j] = m_returnCombine[i - 1][j];
                else
                {
                    if (m_returnCombine[i - 1][j] < s + 1) n[j] = m_returnCombine[i - 1][j] + 1;
                    else
                    {
                        n[j] = m_returnCombine[i - 1][j];
                        m_digit--;
                    }
                }
            }
         */
        return m_returnCombine;
    }
}