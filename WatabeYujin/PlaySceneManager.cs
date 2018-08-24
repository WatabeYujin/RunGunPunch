using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class PlaySceneManager : MonoBehaviour
{

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private Light directionalLight;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Text comboText;
	[SerializeField]
	private AudioSource BGMaudiosource;
	[SerializeField]
	private AudioSource[] SEaudiosource = new AudioSource[3];
    [SerializeField]
    private Color compositeModeLightColor;
    [SerializeField]
    private Condition condition;
    [SerializeField]
    private AudioClip compositeSE;
    [SerializeField]
    private Text[] comandOK = new Text[2];
    [SerializeField]
    private Image[] commandAllow = new Image[2];
    [SerializeField]
    private yazirusimove[] yazi=new yazirusimove[2];
    [SerializeField]
    private Sprite[] numimage;

    static public PlaySceneManager SceneManager;
    private Color baseColor;
    private int score;
    private int combo;
    private float elapsedTime = 0;
	private bool BGMPlay = true;
	private int seID=0;
    private int countUpScore;

    public enum Condition
    {
        None,
        Reverse,
        Composite
    }

    void Awake()
    {
        SceneManager = this;
        Application.targetFrameRate = 60;           //目標FPSを60に設定
        baseColor = directionalLight.color;
    }
    void Update()
    {
        ScoreView();
        ComboView();
        ReverseTime();
    }

    void ScoreView()
    {
        scoreText.text = "Score：" + score;
    }
    void ComboView()
    {
        if (combo <= 1) comboText.text = "";
        else comboText.text = combo + "Combo!!";
    }

    public float GetSpeed()
    {
        if (condition == Condition.Composite)
            return speed / 100;
        return speed / 10;
    }
    void ReverseTime()
    {
        if (GetSetNowCondition == Condition.Reverse)
        {
            if (elapsedTime <= 0.1f)
            {
                yazi[0].yazimove();
                yazi[1].yazimove();
            }
            const float m_durationTime = 20f;
            if (m_durationTime <= elapsedTime)
            {
                GetSetNowCondition = Condition.None;
                elapsedTime = 0;
                return;
            }
            elapsedTime += Time.deltaTime;
        }
    }

    IEnumerator ScoreCountUp()
    {
        for(; countUpScore > 0; countUpScore--)
        {
            score++;
            ScoreView();
            yield return null;
        }
    }

    IEnumerator ScorePopUpEvent()
    {
        const int m_maxfontSize = 40;
        const int m_fontSize = 30;
        scoreText.fontSize = m_maxfontSize;
        for (; ; )
        {
            if (m_fontSize >= scoreText.fontSize) break;
            scoreText.fontSize --;
            yield return null;
        }
        scoreText.fontSize = m_fontSize;
    }

    public Condition GetSetNowCondition
    {
        get
        {
            return condition;
        }
        set
        {
            if (condition == value) return;
            condition = value;
            LightColorChange();
        }
    }

    void LightColorChange()
    {
        if (condition == Condition.Composite) directionalLight.color = compositeModeLightColor;
        else directionalLight.color = baseColor;
    }

    IEnumerator ScoreCountMove(int score,Vector2 scorePos)
    {
		Vector2 m_scorePos = new Vector2 (Screen.width-Screen.width/10, Screen.height-Screen.height/10);
		const float m_moveTime = 0.5f;
        const float m_popmoveTime = 0.3f;
        GameObject m_textObj = new GameObject();
		Destroy (m_textObj, m_moveTime);
        m_textObj.transform.parent = GameObject.Find("Canvas").transform;
        Image m_text= m_textObj.AddComponent<Image>();
        TextImageView(score, m_text);
		RectTransform m_rect = m_textObj.GetComponent<RectTransform>();

        m_rect.position = scorePos;
        Vector3 m_popMove = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));
        m_rect.DOMove(m_rect.transform.position+ m_popMove, m_popmoveTime);
        yield return new WaitForSeconds(m_popmoveTime);

        
		m_rect.DOMove(m_scorePos, m_moveTime);
		yield return new WaitForSeconds (m_moveTime);
    }

    void TextImageView(int score,Image image)
    {
        var digit = score;
        //要素数0には１桁目の値が格納
        List<int> number = new List<int>();
        while (digit != 0)
        {
            score = digit % 10;
            digit = digit / 10;
            number.Add(score);
        }

        image.sprite = numimage[number[0]];
        for (int i = 1; i < number.Count; i++)
        {
            //複製
            RectTransform scoreimage = (RectTransform)Instantiate(image.gameObject).transform;
            scoreimage.SetParent(this.transform, false);
            scoreimage.localPosition = new Vector2(
                scoreimage.localPosition.x - scoreimage.sizeDelta.x * i,
                scoreimage.localPosition.y);
            scoreimage.GetComponent<Image>().sprite = numimage[number[i]];
        }
    }

    IEnumerator ScoreUpIenumerator(int getScore, Vector2 scorePos)
    {
        combo++;
        countUpScore += getScore;
        yield return StartCoroutine(ScoreCountMove(getScore, scorePos));
        StartCoroutine(ScoreCountUp());
        StartCoroutine(ScorePopUpEvent());
    }

    public void ScoreUP(int getScore,Vector2 scorePos){
		StartCoroutine (ScoreUpIenumerator (getScore, scorePos));
    }

    public void ComboStop()
    {
        combo = 0;
    }

	public void BGMisPlay(bool value){
		if (value)
			BGMaudiosource.Play ();
		else
			BGMaudiosource.Pause ();
	}
	public void SEPlay(AudioClip se){
		SEaudiosource [seID].clip = se;
		SEaudiosource [seID].Play ();
        seID++;
        if (seID >= 3) seID = 0;
	}
    public void CompositeSEPlay()
    {
        SEaudiosource[seID].clip = compositeSE;
        SEaudiosource[seID].Play();
        seID++;
        if (seID >= 3) seID = 0;
    }

    public void ComandView(int comand,int playerID)
    {
        switch (comand)
        {
            case 0:
                comandOK[playerID].enabled = true;
                commandAllow[playerID].enabled = false;
                break;
            case 1:
                commandAllow[playerID].enabled = true;
                comandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.forward * 90;
                break;
            case 2:
                commandAllow[playerID].enabled = true;
                comandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.zero;
                break;
            case 3:
                commandAllow[playerID].enabled = true;
                comandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.forward * -90;
                break;
            default:
                commandAllow[playerID].enabled = false;
                comandOK[playerID].enabled = false;
                break;
        }
    }


}
