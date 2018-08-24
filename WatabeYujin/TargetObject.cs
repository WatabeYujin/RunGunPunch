using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TargetObject : MonoBehaviour
{
	[SerializeField]
	private AudioClip breakSE;
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
    [SerializeField]
    private EnchantmentStatus enchantmentStatus;
	[SerializeField]
	private float outsideStartPos = 100.0f; //画面外から来るオブジェクトが生成されてから移動する値
	[SerializeField]
	private Transform stage;   //ステージのオブジェクト
    [SerializeField]
    private Transform presentbox;
	public const float angle = 5.0f; //一秒当たりの回転角度
    private yazirusimove[] yazi = new yazirusimove[2];

    public int compositeCommand1Player = 0;//ボス用のコマンド入力
    public int compositeCommand2Player = 0;//左=1,縦=2,右=3　（例）左左右縦の場合1132
    private Transform testtrans;
	private Vector3 stagePos = new Vector3(0,-200,0); //回転の中心をとるために使う変数
    private RaycastHit hit;
    private float spawnPosX; //オブジェクトの生成された際のx軸
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
        OutsideArea,
        ReverseSide,
        ReverseLane
    }
    public enum EnchantmentStatus
    {
        None = 0,
        Reverse = 1,
    }
    ////////////////////////////////////////////////////////////////////////////////////
	void Start(){
        if (targetMoveType == TargetMoveType.OutsideArea && presentbox == null)
        {
            isMove = false;
            GetComponent<BoxCollider>().enabled = false;
        }
        transform.eulerAngles = new Vector3(0, 180, 0);
		FirstTargetPositionGet ();
	}

    void Update()
    {
        //TargetMove();
        CompositeEvent();
		if(targetMoveType == TargetMoveType.OutsideArea)
		{
			OutsideAreaMove();
		}
        JustBeforeMove();
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

    /// <summary>
    /// 協力ターゲットが居る場合の処理
    /// </summary>
    void CompositeEvent()
    {
        switch (targetType)
        {
            case TargetType.Composite:
                if (!CompositeAreaCheck()) return;
                if (PlaySceneManager.SceneManager.GetSetNowCondition ==
                    PlaySceneManager.Condition.Composite) return;
                PlaySceneManager.SceneManager.GetSetNowCondition = PlaySceneManager.Condition.Composite;
                PlaySceneManager.SceneManager.BGMisPlay(false);
                PlaySceneManager.SceneManager.CompositeSEPlay();
                ComandView(0,true);
                ComandView(1,true);
                break;
            default:
                ColorChange(PlaySceneManager.SceneManager.GetSetNowCondition == PlaySceneManager.Condition.Composite);
                break;
        }
    }

    /// <summary>
    /// 被撃破処理
    /// </summary>
    void TargetBreak()
    {
		ScoreEvent ();
        //撃破時の処理をここに入れる//
        switch (enchantmentStatus)
        {
            case EnchantmentStatus.Reverse:
                //反転状態に変更
                PlaySceneManager.SceneManager.GetSetNowCondition =
                    PlaySceneManager.Condition.Reverse;
                break;
            default:
                break;
        }
        if (targetType == TargetType.Composite)
        {
            StartCoroutine(CompositeBreakEvent());
            return;
        }
        //撃破時の処理をここに入れる//
        DestroyEvent();

    }

    IEnumerator CompositeBreakEvent()
    {
        isMove = false;
        Vector3 m_burstAngle = new Vector3(0.1f, 0.5f, 60);
        const float m_rotateSpeed = 50;
        for(int i = 0; i < 30; i++)
        {
            transform.eulerAngles += Vector3.forward * m_rotateSpeed;
            transform.position += m_burstAngle;
            yield return null;
        }
        DestroyEvent();
    }
    
    void JustBeforeMove()
    {
        const float m_moveRange = 35f;
        if (targetMoveType == TargetMoveType.Nomal) return;
        if (targetMoveType == TargetMoveType.OutsideArea) return;
        if (transform.position.z > m_moveRange) return;
        if (targetMoveType == TargetMoveType.ReverseLane) StartCoroutine(MoveReverseLane());
        if (targetMoveType == TargetMoveType.ReverseSide) StartCoroutine(MoveReverseSide());
    }

    void PlayerAttackEvent()
    {
        PlaySceneManager.SceneManager.ComboStop();
        //ミスの処理をここに入れる//


        //ミスの処理をここに入れる//
        Destroy(gameObject, 0.1f);
        DestroyEvent();
    }

    /// <summary>
    /// 自身の消失処理
    /// </summary>
    void DestroyEvent()
    {
        isMove = false;
        EffectSpawn();
        if (targetType == TargetType.Composite)
        {
            PlaySceneManager.SceneManager.GetSetNowCondition =
                PlaySceneManager.Condition.None;
            PlaySceneManager.SceneManager.BGMisPlay(true);
            ComandView(0, false);
            ComandView(1, false);
        }
        //if (targetType == TargetType.Composite)
        Destroy(gameObject);
    }

    /// <summary>
    /// BreakEffectが設定されていた場合そのエフェクトを生成する。
    /// </summary>
    void EffectSpawn()
    {
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

	void FirstTargetPositionGet(){
        /*
        Transform m_stage = stage;   //targetに、"Sample"の名前のオブジェクトのコンポーネントを見つけてアクセスする

		stagePos = m_stage.position; //変数targetPosにSampleの位置情報を取得
                
		if (targetMoveType == TargetMoveType.Nomal) testtrans.LookAt(m_stage); //自分の向きをターゲットの正面に向ける
        */
        spawnPosX = transform.position.x;
        if (targetMoveType == TargetMoveType.OutsideArea)
		{
			

			Vector3 m_pos = transform.position;

			m_pos.x += outsideStartPos;

			transform.position = m_pos;
		}
        
        //transform.Rotate(new Vector3(0, 0, Random.Range(0, 360)), Space.World); //自分をZ軸を中心に0～360でランダムに回転させる
    }

    /// <summary>
    /// 協力ターゲットが存在する場合、色を変える処理
    /// </summary>
    /// <param name="isCombineMode">協力ターゲットが出現しているか</param>
    void ColorChange(bool isCombineMode)
    {
        Color m_color = new Color(60f / 360f, 60f / 360f, 60f / 360f);
        const string m_baseColorName = "_BaseColor";
        if (isCombineMode) renderer.material.SetColor(m_baseColorName, m_color);
        else renderer.material.SetColor(m_baseColorName, Color.white);
    }

	/// <summary>
	/// オブジェクトのタイプを切り替える
	/// </summary>
	public void TargetTypeChange()
	{

		targetType = (TargetType)Mathf.Abs((int)targetType - 1);

	}

	/// <summary>
	/// 画面外から来るオブジェクトの動き
	/// </summary>
	private void OutsideAreaMove()
	{
        
        if (!isMove) return;
        Debug.Log("OutsideAreaMove()");
        if (outsideStartPos > 0)
		{
            transform.position -= transform.right * PlaySceneManager.SceneManager.GetSpeed()*30;   // オブジェクトをステージに向かって動かす
            Debug.Log(PlaySceneManager.SceneManager.GetSpeed());
            if (transform.position.x <= spawnPosX)   //生成された位置のX座標になったら通常のオブジェクトの動きになる
			{
                transform.position = new Vector3(spawnPosX, transform.position.y,transform.position.z);
                //targetMoveType = TargetMoveType.Nomal;
			}
		}
		else if(outsideStartPos < 0)
		{
            transform.position += transform.right * PlaySceneManager.SceneManager.GetSpeed()*30;
            Debug.Log(PlaySceneManager.SceneManager.GetSpeed());
            if (transform.position.x >= spawnPosX)
			{
                Debug.Log(spawnPosX);
                transform.position = new Vector3(spawnPosX, transform.position.y,transform.position.z);
                //targetMoveType = TargetMoveType.Nomal;

			}
		}        
	}

	/// <summary>
	/// オブジェクトの角度を調節する
	/// </summary>
	private void SlopeAdjust()
	{
		if (!isMove) return;
        
		Vector3 m_axis = transform.TransformDirection(Vector3.right);
		transform.RotateAround(stagePos, m_axis, angle *- PlaySceneManager.SceneManager.GetSpeed()); // オブジェクトの回転
        transform.eulerAngles=new Vector3(0, 180, 0);
        /*
		if (Physics.Raycast(                // Transformの真下の地形の法線を調べる
			transform.position,
			-transform.up,
			out hit,
			float.PositiveInfinity))    
		{

			Quaternion m_q = Quaternion.FromToRotation(     // 傾きの差を求める
				transform.up,
				hit.normal);

            transform.eulerAngles = Vector3.zero;
			//transform.rotation *= m_q;      // 自分を回転させる
		}*/
	}

    void ComandView(int playerID,bool view)
    {
        if (!view)
        {
            PlaySceneManager.SceneManager.ComandView(-1, playerID);
            return;
        }
        int m_viewComand = 0;
        if (playerID == 0)
            m_viewComand = compositeCommand1Player % 10;
        else
            m_viewComand = compositeCommand2Player % 10;
        PlaySceneManager.SceneManager.ComandView(m_viewComand, playerID);
    }

	void ScoreEvent(){
        Vector2 m_targetPos = RectTransformUtility.WorldToScreenPoint(Camera.main, transform.position);

        PlaySceneManager.SceneManager.ScoreUP(scorePoint, m_targetPos);
	}

	void PresentBaloonDamageEvent(){
        if (presentbox != null)
        {
            presentbox.parent = null;
            presentbox.GetComponent<TargetObject>().BaloonMove();
        }
        ScoreEvent();
        TargetBreak();
    }

    /// <summary>
    /// バルーン用の移動処理
    /// </summary>
    /// <returns></returns>
    IEnumerator BaloonMoveIEnumerator()
    {
        float m_movePosY = -2;
        for (int i = 0; i < 10; i++)
        {
            Vector3 m_axis = transform.TransformDirection(Vector3.right);
            transform.RotateAround(stagePos, m_axis, angle * PlaySceneManager.SceneManager.GetSpeed() * 3); // オブジェクトの回転
            transform.position += Vector3.up * m_movePosY / 10;
            yield return null;
        }
        GetComponent<BoxCollider>().enabled = true;
        isMove = true;
    }

    IEnumerator MoveReverseSide()
    {
        Debug.Log("MoveReverseSide");
        targetMoveType = TargetMoveType.Nomal;
        const float m_moveTime = 0.7f;
        isMove = false;
        transform.DOMoveX(-spawnPosX, m_moveTime);
        yield return new WaitForSeconds(m_moveTime);
        isMove = true;
        
    }

    IEnumerator MoveReverseLane()
    {
        isMove = false;
        Debug.Log("MoveReverseLane");
        targetMoveType = TargetMoveType.Nomal;
        const float m_laneDifference = 5f;
        const float m_moveTime = 0.7f;
        isMove = false;
        if (targetType==TargetType.OverArm)
            transform.DOMoveY(transform.position.y-m_laneDifference, m_moveTime);
        else if(targetType == TargetType.EnergyGun)
            transform.DOMoveY(transform.position.y + m_laneDifference, m_moveTime);
        yield return new WaitForSeconds(m_moveTime);
        isMove = true;
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
			if (targetType != TargetType.EnergyGun) return false;
        else if (attackPlayerID == 1)
            if (targetType != TargetType.OverArm) return false;
        else return false;
		if(targetMoveType == TargetMoveType.OutsideArea){
            PresentBaloonDamageEvent ();
			return true;
		}
		TargetBreak();
		return true;
    }

    /// <summary>
    /// 協力ターゲット用のダメージ処理
    /// </summary>
    /// <param name="attackPlayerID">攻撃したプレイヤーのID</param>
    /// <param name="lane">攻撃したレーン</param>
	public void CompositeDamage(int attackPlayerID, int lane)
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
        ComandView(attackPlayerID,true);
        if (compositeCommand1Player != 0 || compositeCommand2Player != 0) return;
        TargetBreak();
    }

    public void StageCenterSet(Transform obj)
    {
        stage = obj;
        FirstTargetPositionGet();
    }

    public void BaloonMove()
    {
        StartCoroutine(BaloonMoveIEnumerator());
    }
}
