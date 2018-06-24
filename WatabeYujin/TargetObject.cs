using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TargetObject : MonoBehaviour {
    [SerializeField]
    public GameObject breakEffect; //ターゲット破壊時に生成するエフェクトのオブジェクト
    [SerializeField]
    public TargetType targetType;  //自身のターゲットの種類(enum)
    [SerializeField]
    public TargetMoveType targetMoveType;
    [SerializeField]
    public Renderer renderer;
	[SerializeField]
	public int scorePoint = 1;
    
    public int compositeCommand1Player=0;//左=1,縦=2,右=3
    public int compositeCommand2Player=0;

    private bool isMove = true;     //移動しているか否か

    public enum TargetType         //ターゲットの種類
    {
        EnergyGun = 0,
        OverArm = 1,
        Composite = 2
    }
    public enum TargetMoveType
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
		PlaySceneManager.SceneManager.ScoreUP (scorePoint);
        //バフ系の処理をここに入れる//


        //バフ系の処理をここに入れる//
        DestroyEvent();

    }

    void PlayerAttackEvent()
    {
		PlaySceneManager.SceneManager.ComboStop ();
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
        if (breakEffect)
        {
            Transform m_effectTrans = Instantiate(breakEffect).transform;
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
        const float m_range = 35;
        return m_range >= transform.position.z;
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

	public void CompositeDamage(int attackPlayerID,int lane)
	{
        Debug.Log(attackPlayerID);
		if (attackPlayerID == 0)
		{
			if (compositeCommand1Player == 0) return;
			if (compositeCommand1Player % 10 != lane) return;
			compositeCommand1Player /= 10;
		}
		else if (attackPlayerID == 1)
		{
			if (compositeCommand2Player == 0) return;
			if (compositeCommand2Player % 10 != lane) return;
			compositeCommand2Player /= 10;
		}
        if (compositeCommand1Player != 0 || compositeCommand2Player != 0) return;
        TargetBreak();
	}
}
