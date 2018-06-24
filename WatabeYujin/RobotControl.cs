using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] centerShotTransform =
        new Transform[2];                       //中央の攻撃判定の位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] rightShotTransform =
        new Transform[2];                       //右の攻撃判定の位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] leftShotTransform =
        new Transform[2];                       //左の攻撃判定の位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private LineRenderer lineRenderer;          //攻撃時に表示するラインのLineRenderer
    [SerializeField]
    private Material[] lineMaterials=
        new Material[2];                        //攻撃時表示するLineRendererのMaterial
    [SerializeField]
    private float attackDistance = 15;          //最大射程

    IEnumerator lineDeleteIEnumerator;

    public enum Lane                            //攻撃レーンの位置
    {
        Left = 1,
        Center = 2,
        Right = 3
    }

    ///////////////////////////////////////////////////////////////////////////////////
    void Start() {
        lineDeleteIEnumerator = LineRendererDelete();
        
    }

	void Update(){
        PcJoyConControl();
    }

    /// <summary>
    /// PCでのJoyConを振る入力を行う。
    /// 1P：A=左振り , S=縦振り , D=右振り
    /// 2P：方向キー←=左振り , 方向キー↓=縦振り , 方向キー→=右振り
    /// </summary>
    void PcJoyConControl() {
        if (Input.GetKeyDown(KeyCode.A))
            Attack(0, Lane.Left);
        if (Input.GetKeyDown(KeyCode.S))
            Attack(0, Lane.Center);
        if (Input.GetKeyDown(KeyCode.D))
            Attack(0, Lane.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            Attack(1, Lane.Left);
        if (Input.GetKeyDown(KeyCode.DownArrow))
            Attack(1, Lane.Center);
        if (Input.GetKeyDown(KeyCode.RightArrow))
            Attack(1, Lane.Right);
    }

    /// <summary>
    /// 設定した攻撃判定の場所からRayを飛ばして命中判定を行う
    /// </summary>
    /// <param name="shotTransform">Rayを飛ばすTransform</param>
    /// <returns><para>命中したTargetタグのGameObject</para><para>（命中しなかった、あるいは別のタグだった場合nullを返す）</para></returns>
    private GameObject AttackRayCast(Transform shotTransform)
    {
        Ray m_attackRay = new Ray(shotTransform.position, shotTransform.forward);
        RaycastHit m_hit;                                 //レイの距離
        const string m_targetTagName = "Target";
        if (!Physics.Raycast(m_attackRay, out m_hit, attackDistance))
            return null;
        if (m_hit.collider.tag != m_targetTagName)
            return null;
        return m_hit.collider.gameObject;
    }

    /// <summary>
    /// 協力ターゲット存在時の攻撃処理
    /// </summary>
    /// <param name="playerID">プレイヤーのID</param>
    /// <param name="attackLane">攻撃するレーンの位置(enum)</param>
    void CompositeAttack(int playerID, Lane attackLane)
    {
        Transform m_shotTransform = centerShotTransform[playerID];
        GameObject m_hitTargetObject = AttackRayCast(m_shotTransform);                     //Rayによる命中判定を行う
        if (m_hitTargetObject != null)
        {
            m_hitTargetObject.GetComponent<TargetObject>().CompositeDamage(playerID, (int)attackLane);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_hitTargetObject.transform.position);
        }
        else
        {
            Vector3 m_endPosition = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_endPosition);
        }
    }

    private void RayView(Material lineMaterial, Vector3 shotPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.SetPosition(0, shotPosition);
        lineRenderer.SetPosition(1, endPosition);
        StopCoroutine(lineDeleteIEnumerator);
        StartCoroutine(lineDeleteIEnumerator);
    }

    private IEnumerator LineRendererDelete()
    {
        const float m_deleteTime = 0.5f;
        yield return new WaitForSeconds(m_deleteTime);
        lineRenderer.enabled = false;
    }

    ///////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// プレイヤーの攻撃の処理を行う
    /// </summary>
    /// <param name="playerID">プレイヤーのID</param>
    /// <param name="attackLane">攻撃するレーンの位置(enum)</param>
    public void Attack(int playerID, Lane attackLane)
    {
        if (PlaySceneManager.SceneManager.GetSetCompositeMode) {
            CompositeAttack(playerID, attackLane);
            return;
        }
        GameObject m_hitTargetObject = null;
        Transform m_shotTransform = null;
        if (attackLane == Lane.Center)
            m_shotTransform = centerShotTransform[playerID];
        if (attackLane == Lane.Right)
            m_shotTransform = rightShotTransform[playerID];
        if (attackLane == Lane.Left)
            m_shotTransform = leftShotTransform[playerID];

        m_hitTargetObject = AttackRayCast(m_shotTransform);                     //Rayによる命中判定を行う

        if (m_hitTargetObject != null)
        {
            m_hitTargetObject.GetComponent<TargetObject>().Damage(playerID);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_hitTargetObject.transform.position);
        }
        else
        {
            Vector3 m_endPosition = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
            RayView(lineMaterials[playerID], m_shotTransform.position, m_endPosition);
        }
    }

    /// <summary>
    /// 上に乗ってるパイロットたちを動かす処理
    /// </summary>
    /// <param name="playerID">プレイヤーのid</param>
    /// <param name="axis">スティックの入力</param>
    public void Pilot(int playerID, Vector2 axis)
    {

    }


}
