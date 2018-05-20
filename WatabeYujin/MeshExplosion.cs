using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MeshExplosion : MonoBehaviour {
    //オブジェクトをバラバラにして吹き飛ばすスクリプト

    public void Explode(Transform target,Vector3 center)
    {
        Debug.Log("我が名はめぐみん!");
        Debug.Log("紅魔族随一の魔法の使い手にして、爆裂魔法を操りし者。");
        Debug.Log("我が力、見るがいい！");
        Debug.Log("『エクスプロージョン』！!");

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
        m_objcount = Mathf.CeilToInt((float)m_objcount / 30);
        for (int i = 0; i < triangles.Length; i += 3 * m_objcount)
        {
            // TODO: inherit speed, spin...?
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 s = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((s.x + s.y + s.z) / 3) * 0.3f;
            List<Vector3> m_verticesList=new List<Vector3>();
            for(int j = 0; j < 3 * m_objcount; j++)
            {
                m_verticesList.Add(vertices[i + j]);
            }
            Rigidbody r = CreateMeshPiece(extrudeSize, target.transform.position, target.GetComponent<Renderer>().material, index, averageNormal, m_verticesList, uvs[triangles[i]], uvs[triangles[i + 1]], uvs[triangles[i + 2]]);
            Vector3 testt = (vertices[triangles[i]] + vertices[triangles[i + 1]] + vertices[triangles[i + 2]])/3;
            r.AddForce((testt- aaa.transform.localPosition) * 5,ForceMode.VelocityChange);
            index++;
        }
        Debug.Log(index);
        // destroy original
        Destroy(target.gameObject);
    }

    Rigidbody CreateMeshPiece(float extrudeSize, Vector3 pos, Material mat, int index, Vector3 faceNormal, List<Vector3> v, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {

        GameObject go = new GameObject();
        go.name = "piece_" + index;
        Mesh mesh = go.AddComponent<MeshFilter>().mesh;
        go.AddComponent<MeshRenderer>();
        //go.tag = "Explodable"; // set this only if should be able to explode this piece also
        go.GetComponent<Renderer>().material = mat;
        go.transform.position = pos;

        Vector3[] vertices = new Vector3[3 * (v.Count + 1)];
        int[] triangles = new int[3 * (v.Count + 1)];
        Vector2[] uvs = new Vector2[3 * (v.Count + 1)];
        
        Vector3 m_vcenter=Vector3.zero;
        for(int i = 0; i < v.Count; i++)
        {
            m_vcenter += v[i];
        }
        m_vcenter /= v.Count;

        m_vcenter = m_vcenter + (-faceNormal) * extrudeSize;
        v.Add(m_vcenter);

        Debug.Log("vertices.Length" + vertices.Length);
        Debug.Log("v,length" + v.Count);
        int m_vcount = 0;
        for (int i = 0; i < vertices.Length-9; i+=3)
        {
            for (int j = 0; j < 3; j++)
            {
                vertices[i + j] = v[i - v.Count * (m_vcount / v.Count)];
                vertices[i + j] = v[j + 1 - v.Count * ( m_vcount / v.Count)];
                vertices[i + j] = v[j + 2 - v.Count * (m_vcount / v.Count)];
                m_vcount+=3;
                Debug.Log("count" + m_vcount);
                Debug.Log("(v.Count * m_vcount / v.Count)" + v.Count * (m_vcount / v.Count));
            }
        }
        Debug.Log("count" + m_vcount);
        for(int i = 0;i< v.Count; i+=6)
        {
            triangles[0 + i] = 0 + i;
            triangles[1 + i] = 1 + i;
            triangles[2 + i] = 2 + i;

            triangles[3 + i] = 5 + i;
            triangles[4 + i] = 4 + i;
            triangles[5 + i] = 3 + i;
        }

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
}
