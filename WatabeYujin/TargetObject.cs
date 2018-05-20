using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour {
    [SerializeField]
    private GameObject BreakEffect; //ターゲット破壊時に生成するエフェクトのオブジェクト
    [SerializeField]
    private TargetType targetType;  //自身のターゲットの種類(enum)
    [SerializeField]
    private Rigidbody thisRigidbody;//自身のrigidBody

    private float speed = 1;        //オブジェクトの移動スピード
    private bool isMove = true;     //移動しているか否か

    private enum TargetType         //ターゲットの種類
    {
        EnergyGun = 0,
        OverArm = 1,
        Composite = 2
    }

    ////////////////////////////////////////////////////////////////////////////////////

    void Update () {
        TargetMove();
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    void TargetMove()
    {
        if (!isMove) return;
        transform.position = transform.forward * speed;
    }

    /// <summary>
    /// 被撃破処理
    /// </summary>
    void TargetBreak()
    {
        //バフ系の処理をここに入れる//


        //バフ系の処理をここに入れる//
        DestroyEvent();

    }

    /// <summary>
    /// 自身の消失処理
    /// </summary>
    void DestroyEvent()
    {
        isMove = false;
        EffectSpawn();
        Destroy(gameObject);
    }

    /// <summary>
    /// BreakEffectが設定されていた場合そのエフェクトを生成する。
    /// </summary>
    void EffectSpawn() {
        if (BreakEffect)
        {
            Transform m_effectTrans = Instantiate(BreakEffect).transform;
            m_effectTrans.position = transform.position;
        }
    }

    ////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// ダメージを与えた時の処理
    /// </summary>
    /// <param name="attackPlayerID">攻撃したプレイヤーのID</param>
    /// <returns>ターゲットを壊した時はtrueを返す</returns>
    public bool Damage(int attackPlayerID)
    {
        if (attackPlayerID == 0)
        {
            if (targetType == TargetType.OverArm) return false;
            TargetBreak();
            return true;
        }
        else if (attackPlayerID == 1)
        {
            if (targetType == TargetType.EnergyGun) return false;
            TargetBreak();
            return true;
        }
        else return false;
    }
}
