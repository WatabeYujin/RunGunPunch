using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotControl : MonoBehaviour
{
    [SerializeField]
    private Transform[] centerRayTransform =
        new Transform[2];                       //中央の攻撃Rayの位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] rightRayTransform =
        new Transform[2];                       //右の攻撃Rayの位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] leftRayTransform =
        new Transform[2];                       //左の攻撃Rayの位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private LineRenderer lineRenderer;          //攻撃時に表示するラインのLineRenderer
    [SerializeField]
    private Material[] lineMaterials =
        new Material[2];                        //攻撃時表示するLineRendererのMaterial
    [SerializeField]
    private float attackDistance = 15;          //最大射程
    [SerializeField]
    private GameObject[] bulletObj = new GameObject[2];
    IEnumerator lineDeleteIEnumerator;
    [SerializeField]
    private AudioSource overSE;
    [SerializeField]
    private AudioSource energySE;
    [SerializeField]
    private Animator[] armAnims = new Animator[2];
    [SerializeField]
    private Transform[] shotTransform = new Transform[2];

    public enum Lane                            //攻撃レーンの位置
    {
        Left = 1,
        Center = 2,
        Right = 3
    }

    ///////////////////////////////////////////////////////////////////////////////////

    void Start()
    {
        lineDeleteIEnumerator = LineRendererDelete();
    }

    void Update()
    {
        PcJoyConControl();
    }

    /// <summary>
    /// PCでのJoyConを振る入力を行う。
    /// 1P：A=左振り , S=縦振り , D=右振り
    /// 2P：方向キー←=左振り , 方向キー↓=縦振り , 方向キー→=右振り
    /// </summary>
    void PcJoyConControl()
    {
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
        Transform m_shotTransform = centerRayTransform[playerID];
        GameObject m_hitTargetObject = AttackRayCast(m_shotTransform);//Rayによる命中判定を行う        
        
        if (m_hitTargetObject != null)
        {
            m_hitTargetObject.GetComponent<TargetObject>().CompositeDamage(playerID, (int)attackLane);

        }
        else
        {
            Vector3 m_endPosition = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
        }
    }
    private void RayView(Material lineMaterial, Vector3 shotPosition, Vector3 endPosition)
    {
        lineRenderer.enabled = true;
        lineRenderer.material = lineMaterial;
        lineRenderer.SetPosition(0, shotPosition);
        lineRenderer.SetPosition(1, endPosition);
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
        ShotAnim(playerID, attackLane);
        if (PlaySceneManager.SceneManager.GetSetNowCondition ==
            PlaySceneManager.Condition.Reverse)
            playerID = playerID == 0 ? 1 : 0;
        if (PlaySceneManager.SceneManager.GetSetNowCondition ==
            PlaySceneManager.Condition.Composite)
        {
            CompositeAttack(playerID, attackLane);
            return;
        }
        if (playerID == 0) energySE.Play();
        else overSE.Play();
        GameObject m_hitTargetObject = null;
        Transform m_shotTransform = null;
        if (attackLane == Lane.Center)
            m_shotTransform = centerRayTransform[playerID];
        if (attackLane == Lane.Right)
            m_shotTransform = rightRayTransform[playerID];
        if (attackLane == Lane.Left)
            m_shotTransform = leftRayTransform[playerID];

        m_hitTargetObject = AttackRayCast(m_shotTransform);                     //Rayによる命中判定を行う
        Debug.Log("m_hitTargetObject" + m_hitTargetObject);
        if (m_hitTargetObject != null)
        {
            StartCoroutine(BulletSpawn(m_hitTargetObject.transform, playerID));
        }
        else
        {
            GameObject m_endPosition = new GameObject();
            m_endPosition.transform.position = m_shotTransform.position + (m_shotTransform.transform.forward * attackDistance);
            Destroy(m_endPosition, 3);
            StartCoroutine(BulletSpawn(m_endPosition.transform, playerID));
        }
        
    }

    void ShotAnim(int armID,Lane shotPos)
    {
        const string m_left = "LeftShot";
        const string m_center = "CenterShot";
        const string m_right = "RightShot";

        switch (shotPos)
        {
            case Lane.Left:
                armAnims[armID].SetTrigger(m_left);
                break;
            case Lane.Center:
                armAnims[armID].SetTrigger(m_center);
                break;
            case Lane.Right:
                armAnims[armID].SetTrigger(m_right);
                break;
            default:
                break;
        }
    }

    IEnumerator BulletSpawn(Transform hitPos,int playerID)
    {
        Transform m_shotPos = shotTransform[playerID];
        if (playerID == 0) yield return new WaitForSeconds(0.15f);
        GameObject m_obj = Instantiate(bulletObj[playerID]);
        m_obj.transform.position = m_shotPos.position;
        m_obj.GetComponent<HomingBullet>().TargetTransformSet(hitPos);
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