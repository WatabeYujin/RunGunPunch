using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour {
    [SerializeField]
    private GameObject BreakEffect; //ターゲット破壊時に生成するエフェクトのオブジェクト
    [SerializeField]
    private TargetType targetType;  //自身のターゲットの種類(enum)
    [SerializeField]
    private TargetMoveType targetMoveType;
    [SerializeField]
    private Rigidbody thisRigidbody;//自身のrigidBody
    [SerializeField]
    private Renderer renderer;

    private bool isMove = true;     //移動しているか否か

    private enum TargetType         //ターゲットの種類
    {
        EnergyGun = 0,
        OverArm = 1,
        Composite = 2
    }
    private enum TargetMoveType
    {
        Nomal,
        OutsideArea
    }


    ////////////////////////////////////////////////////////////////////////////////////

    void Update () {
        TargetMove();
        CompositeEvent();
    }

    /// <summary>
    /// 移動処理
    /// </summary>
    void TargetMove()
    {
        if (!isMove) return;
        transform.position += transform.forward * PlaySceneManager.SceneManager.GetSpeed();
    }

    void CompositeEvent()
    {
        switch (targetType)
        {
            case TargetType.Composite:
                if (!CompositeAreaCheck()) return;
                CompositeModeChange(true);
                break;
            default:
                if (!PlaySceneManager.SceneManager.GetSetCompositeMode) return;
                ColorChange();
                break;
        }
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

    void PlayerAttackEvent()
    {
        //デバフ系の処理をここに入れる//


        //デバフ系の処理をここに入れる//
        DestroyEvent();
    }

    /// <summary>
    /// 自身の消失処理
    /// </summary>
    void DestroyEvent()
    {
        isMove = false;
        EffectSpawn();
        if (targetType == TargetType.Composite) CompositeModeChange(false);
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

    void OnTriggerEnter(Collider col)
    {
        const string m_playerTag = "Player";
        if (col.tag != m_playerTag) return;
        PlayerAttackEvent();
    }

    bool CompositeAreaCheck()
    {
        const float m_range = 15;
        return m_range <= transform.position.z;
    }

    void CompositeModeChange(bool value)
    {
        PlaySceneManager.SceneManager.GetSetCompositeMode = value;
    }

    void ColorChange()
    {
        
        Color m_color = new Color(60f/360f,60f/360f,60f/360f);
        const string m_baseColorName = "_BaseColor";
        renderer.material.SetColor(m_baseColorName, m_color);
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
