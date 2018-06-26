using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

	public int compositeCommand1Player; //左=0,縦=1,右=2
	public int compositeCommand2Player; 

    private bool isMove = true;     //移動しているか否か

    public float angle = 5f; //一秒当たりの回転角度

    private Vector3 stagePos; //回転の中心をとるために使う変数

    private RaycastHit hit;

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

    void Start()
    {
        //targetに、"Sample"の名前のオブジェクトのコンポーネントを見つけてアクセスする
        Transform m_stage = GameObject.Find("Sphere").transform;
        //変数targetPosにSampleの位置情報を取得
        stagePos = m_stage.position;

        //自分の向きをターゲットの正面に向ける
        transform.LookAt(m_stage);

        //自分をZ軸を中心に0～360でランダムに回転させる
        //transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)), Space.World);
    }

    void Update () {
        //TargetMove();
        CompositeEvent();
        SlopeAdjust();
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
	}

    /// <summary>
    /// オブジェクトの角度を調節する
    /// </summary>
    private void SlopeAdjust()
    {
        if (!isMove) return;

        //Sampleを中心に自分を現在の上方向に、毎秒angle分だけ回転する。
        Vector3 m_axis = transform.TransformDirection(Vector3.right);
        transform.RotateAround(stagePos, m_axis, angle * PlaySceneManager.SceneManager.GetSpeed());

        // Transformの真下の地形の法線を調べる
        if (Physics.Raycast(
                    transform.position,
                    -transform.up,
                    out hit,
                    float.PositiveInfinity))
        {
            // 傾きの差を求める
            Quaternion m_q = Quaternion.FromToRotation(
                        transform.up,
                        hit.normal);

            // 自分を回転させる
            transform.rotation *= m_q;
        }
    }


}
