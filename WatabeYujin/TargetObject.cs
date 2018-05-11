using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour {
    [SerializeField]
    private GameObject BreakEffect; //ターゲット破壊時に生成するエフェクトのオブジェクト
    [SerializeField]
    private TargetType targetType;  //ターゲットの種類
    [SerializeField]
    private Rigidbody thisRigidbody;//自身のrigidBody

    private float speed = 1;        //オブジェクトの移動スピード
	private enum TargetType　{
        EnergyGun,
        OverArm,
        Composite
    }
    private bool isMove = true;     //移動しているか否か

    ////////////////////////////////////////////////////////////////////////////////////


    void Update () {
        TargetMove();

    }

    void TargetMove()
    {
        if (!isMove) return;
        transform.position = transform.forward * speed;
    }

    void TargetBreak()
    {
        //バフ系の処理//
        DestroyEvent();

    }

    void DestroyEvent()
    {
        isMove = false;
        EffectSpawn();
        Destroy(gameObject);
    }

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
