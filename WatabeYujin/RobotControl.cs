using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotControl : MonoBehaviour {
    [SerializeField]
    private Transform[] centerShotTransform = 
        new Transform[2];                       //中央の攻撃判定の位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] rightShotTransform =
        new Transform[2];                       //右の攻撃判定の位置（配列0＝1P,配列1＝2P）
    [SerializeField]
    private Transform[] leftShotTransform =
        new Transform[2];                       //左の攻撃判定の位置（配列0＝1P,配列1＝2P）

    public enum Lane                            //攻撃レーンの位置
    {
        Center = 0,
        Left = 1,
        Right = 2
    }

    ///////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// プレイヤーの攻撃の処理を行う
    /// </summary>
    /// <param name="playerID">プレイヤーのID</param>
    /// <param name="attackLane">攻撃するレーンの位置(enum)</param>
    public void Attack(int playerID, Lane attackLane)
    {
        GameObject m_hitTargetObject=null;

        if (attackLane == Lane.Center)
            m_hitTargetObject = AttackRayCast(centerShotTransform[playerID]);
        if (attackLane == Lane.Right)
            m_hitTargetObject = AttackRayCast(rightShotTransform[playerID]);
        if (attackLane == Lane.Left)
            m_hitTargetObject = AttackRayCast(leftShotTransform[playerID]);

        if (m_hitTargetObject != null)
            m_hitTargetObject.GetComponent<TargetObject>().Damage(playerID);
    }

    /// <summary>
    /// 上に乗ってるパイロットたちを動かす処理
    /// </summary>
    /// <param name="playerID">プレイヤーのid</param>
    /// <param name="axis">スティックの入力</param>
    public void Pilot(int playerID, Vector2 axis)
    {

    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// 設定した攻撃判定の場所からRayを飛ばして命中判定を行う
    /// </summary>
    /// <param name="shotTransform">Rayを飛ばすTransform</param>
    /// <returns><para>命中したTargetタグのGameObject</para><para>（命中しなかった、あるいは別のタグだった場合nullを返す）</para></returns>
    private GameObject AttackRayCast(Transform shotTransform)
    {
        Ray m_attackRay = new Ray(shotTransform.position, shotTransform.forward);
        RaycastHit m_hit;
        const float m_attackDistance = 10f;                                         //レイの距離
        const string m_targetTagName = "Target";
        if (!Physics.Raycast(m_attackRay, out m_hit, m_attackDistance))
            return null;
        if (m_hit.collider.tag != m_targetTagName)
            return null;
        return m_hit.collider.gameObject;
            
    }


    
}
