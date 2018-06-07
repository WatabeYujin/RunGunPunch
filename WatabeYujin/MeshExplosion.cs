using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Linq;

public class MeshExplosion : MonoBehaviour {
    //オブジェクトをバラバラにして吹き飛ばすスクリプト

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
        int m_objcount = triangles.Length % 30;
        Debug.Log(m_objcount);
        for (int i = 0; i < triangles.Length; i += 3 + m_objcount)
        //for (int i = 0; i < triangles.Length; i += 3)
        {
            // TODO: inherit speed, spin...?
            Vector3 averageNormal = (normals[triangles[i]] + normals[triangles[i + 1]] + normals[triangles[i + 2]]).normalized;
            Vector3 s = target.GetComponent<Renderer>().bounds.size;
            float extrudeSize = ((s.x + s.y + s.z) / 3) * 0.3f;

            Rigidbody r = CreateMeshPiece(extrudeSize, target.transform.position, target.GetComponent<Renderer>().material, index, averageNormal, vertices[triangles[i]], vertices[triangles[i + 1]], vertices[triangles[i + 2]], uvs[triangles[i]], uvs[triangles[i + 1]], uvs[triangles[i + 2]]);
            
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

    Rigidbody CreateMeshPiece(float extrudeSize, Vector3 pos, Material mat, int index, Vector3 faceNormal, Vector3 v1, Vector3 v2, Vector3 v3, Vector2 uv1, Vector2 uv2, Vector2 uv3)
    {

        GameObject go = new GameObject();
        go.name = "piece_" + index;
        Mesh mesh = go.AddComponent<MeshFilter>().mesh;
        go.AddComponent<MeshRenderer>();
        //go.tag = "Explodable"; // set this only if should be able to explode this piece also
        go.GetComponent<Renderer>().material = mat;
        go.transform.position = pos;

        Vector3[] vertices = new Vector3[3 * 4];
        int[] triangles = new int[3 * 4];
        Vector2[] uvs = new Vector2[3 * 4];

        // get centroid
        Vector3 v4 = (v1 + v2 + v3) / 3;
        // extend to backwards
        v4 = v4 + ((-faceNormal) * extrudeSize)/2;

        // not shared vertices
        // orig face
        //vertices[0] = (v1);
        vertices[0] = (v1);
        vertices[1] = (v2);
        vertices[2] = (v3);
        // right face
        vertices[3] = (v1);
        vertices[4] = (v2);
        vertices[5] = (v4);
        // left face
        vertices[6] = (v1);
        vertices[7] = (v3);
        vertices[8] = (v4);
        // bottom face
        vertices[9] = (v2);
        vertices[10] = (v3);
        vertices[11] = (v4);

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

public static class Combination
{
    public static IEnumerable<T[]> CombinationEnumerate<T>(IEnumerable<T> items, int combinationCount)
    {
        if (combinationCount == 1)
        {
            foreach (var item in items)
                yield return new T[] { item };
            yield break;
        }
        foreach (var item in items)
        {
            var leftside = new T[] { item };

            // item よりも前のものを除く （順列と組み合わせの違い)
            // 重複を許さないので、unusedから item そのものも取り除く
            var unused = items.SkipWhile(e => !e.Equals(item)).Skip(1).ToList();

            foreach (var rightside in CombinationEnumerate(unused, combinationCount - 1))
            {
                yield return leftside.Concat(rightside).ToArray();
            }
        }
    }
}