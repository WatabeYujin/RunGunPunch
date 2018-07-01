using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class MeshExplosion : MonoBehaviour {
    //オブジェクトをバラバラにして吹き飛ばすスクリプト
    [SerializeField]
    int maxTriangles = 30;
    static public MeshExplosion meshExplosion;
    private void Start()
    {
        meshExplosion = this;
    }

    public void Explode(Transform target,Vector3 center,Vector3 moveVerocity)
    {

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
        m_objcount = Mathf.Clamp(m_objcount, 1, maxTriangles);
        m_objcount = 1;
        Debug.Log("m_objcount"+m_objcount);
        //for (int i = 0; i < triangles.Length; i += 3)
        for (int i = 0; i < triangles.Length; i += 3 * m_objcount)
        {
            // TODO: inherit speed, spin...?
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 s = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((s.x + s.y + s.z) / 3) * 0.3f;
            List<Vector3> m_verticesList = new List<Vector3>();
            List<Vector2> m_uvList = new List<Vector2>();
            //for (int j = 0; j < 3 * m_objcount; j++)
            for (int j = 0; j < 3 * m_objcount; j++)
            {
                if (vertices[triangles[i + j]] == null) continue;
                m_verticesList.Add(vertices[triangles[i + j]]);
                m_uvList.Add(uvs[triangles[i + j]]);
            }
            Rigidbody r = CreateMeshPiece(extrudeSize, target.transform.position, target.GetComponent<Renderer>().material, index, averageNormal, m_verticesList, m_uvList);
            
            Vector3 testt = (vertices[triangles[i]] + vertices[triangles[i + 1]] + vertices[triangles[i + 2]]) / 3;
            
            r.AddForce((testt - aaa.transform.localPosition) * 10+ moveVerocity, ForceMode.VelocityChange);
            r.transform.localScale = m_targetScale;
            if (index >= maxTriangles) break;
            index++;
        }
        Debug.Log(index);
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
        int m_combineCount = CombineCount(verticeList.Count+1, 3);
        Vector3[] vertices = new Vector3[3 * m_combineCount];
        int[] triangles = new int[3 * m_combineCount];
        Vector2[] uvs = new Vector2[3 * m_combineCount];

        // get centroid

        Vector3 m_verticeCemter = Vector3.zero;
        foreach(Vector3 verticesPos in verticeList)
        {
            m_verticeCemter += verticesPos;
        }
        m_verticeCemter /= verticeList.Count;
        m_verticeCemter = m_verticeCemter + ((-faceNormal) * extrudeSize) / 2;
        verticeList.Add(m_verticeCemter);

        List<int[]> m_CombineArray = CombineSet(verticeList.Count,m_combineCount);
		
		for(int i = 0;i< m_combineCount * 3;i+=3)
		{
			vertices[i] = verticeList[m_CombineArray[i/3][0]];
			vertices[i+1] = verticeList[m_CombineArray[i / 3][1]];
			vertices[i+2] = verticeList[m_CombineArray[i / 3][2]];
		}
        for (int i = 0; i < triangles.Length; i+=6) {
			triangles [i] = i;
            triangles [i+1] = i+1;
            triangles [i+2] = i+2;
            if (i + 3 >= triangles.Length) break;
            triangles[i + 3] = i + 5;
            triangles[i + 4] = i + 4;
            triangles[i + 5] = i + 3;
        }
        // orig face
        uvs[0] = uvList[0];
        uvs[1] = uvList[1];
        uvs[2] = uvList[2]; // todo
                      // right face
        uvs[3] = uvList[0];
        uvs[4] = uvList[1];
        uvs[5] = uvList[2]; // todo

        // left face
        uvs[6] = uvList[0];
        uvs[7] = uvList[1];
        uvs[8] = uvList[2];   // todo
                        // bottom face (mirror?) or custom color? or fixed from uv?
        uvs[9] = uvList[0];
        uvs[10] = uvList[1];
        uvs[11] = uvList[2]; // todo
        /*
        for (int i = 0; i < m_combineCount * uvList.Count; i += uvList.Count)
        {
            uvs[i] = uvList[0];
            uvs[i+1] = uvList[1];
            uvs[i+2] = uvList[2];
            
            for(int j = 0; j < uvList.Count; j++)
            {
                uvs[i+j] = uvList[j];
            }
        }
        */
        mesh.vertices = vertices;
        mesh.uv = uvs;
		mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        CalculateMeshTangents(mesh);
        MeshCollider mc = go.AddComponent<MeshCollider>();

        mc.sharedMesh = mesh;
        mc.convex = true;
        return go.AddComponent<Rigidbody>();

        //go.AddComponent<MeshFader>();
    }
    void CalculateMeshTangents(Mesh mesh)
    {
        //speed up math by copying the mesh arrays
        int[] triangles = mesh.triangles;
        Vector3[] vertices = mesh.vertices;
        Vector2[] uv = mesh.uv;
        Vector3[] normals = mesh.normals;

        //variable definitions
        int triangleCount = triangles.Length;
        int vertexCount = vertices.Length;

        Vector3[] tan1 = new Vector3[vertexCount];
        Vector3[] tan2 = new Vector3[vertexCount];

        Vector4[] tangents = new Vector4[vertexCount];

        for (long a = 0; a < triangleCount; a += 3)
        {
            long i1 = triangles[a + 0];
            long i2 = triangles[a + 1];
            long i3 = triangles[a + 2];

            Vector3 v1 = vertices[i1];
            Vector3 v2 = vertices[i2];
            Vector3 v3 = vertices[i3];

            Vector2 w1 = uv[i1];
            Vector2 w2 = uv[i2];
            Vector2 w3 = uv[i3];

            float x1 = v2.x - v1.x;
            float x2 = v3.x - v1.x;
            float y1 = v2.y - v1.y;
            float y2 = v3.y - v1.y;
            float z1 = v2.z - v1.z;
            float z2 = v3.z - v1.z;

            float s1 = w2.x - w1.x;
            float s2 = w3.x - w1.x;
            float t1 = w2.y - w1.y;
            float t2 = w3.y - w1.y;

            float r = 1.0f / (s1 * t2 - s2 * t1);

            Vector3 sdir = new Vector3((t2 * x1 - t1 * x2) * r, (t2 * y1 - t1 * y2) * r, (t2 * z1 - t1 * z2) * r);
            Vector3 tdir = new Vector3((s1 * x2 - s2 * x1) * r, (s1 * y2 - s2 * y1) * r, (s1 * z2 - s2 * z1) * r);

            tan1[i1] += sdir;
            tan1[i2] += sdir;
            tan1[i3] += sdir;

            tan2[i1] += tdir;
            tan2[i2] += tdir;
            tan2[i3] += tdir;
        }

        for (int a = 0; a < vertexCount; ++a)
        {
            Vector3 n = normals[a];
            Vector3 t = tan1[a];
            Vector3.OrthoNormalize(ref n, ref t);
            tangents[a].x = t.x;
            tangents[a].y = t.y;
            tangents[a].z = t.z;
            tangents[a].w = (Vector3.Dot(Vector3.Cross(n, t), tan2[a]) < 0.0f) ? -1.0f : 1.0f;
        }
        mesh.tangents = tangents;
    }

    private List<int[]> CombineSet(int listLength,int combineCount)
    {

        const int m_combineCount = 3;
        List<int[]> m_returnCombine =new List<int[]>();
        m_returnCombine.Add(new int[3] { 0, 1, 2 });
        for (int i = 1; i < combineCount; i++)
        {
            int[] n = new int[m_combineCount];
            int m_digit = 0;
            for (int j = m_combineCount - 1; j >= 0; j--)
            {
                n[j] = m_returnCombine[i - 1][j];
                if(2-m_digit==j && m_returnCombine[i - 1][j] < listLength-1-m_digit) n[j]++;
                else m_digit++;
            }
            m_returnCombine.Add(n);
        }
        return m_returnCombine;
    }
	private int CombineCount(int count,int combineCount){
		int n = count;
		int r = combineCount;
		for(int i = 1; i < combineCount; i++)
		{
			n *= count - i;
			r *= combineCount - i;
		}
		return n/r;
	}
}