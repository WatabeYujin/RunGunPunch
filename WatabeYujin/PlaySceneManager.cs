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
    private Text[] commandOK = new Text[2];
    [SerializeField]
    private Image[] commandAllow = new Image[2];
    [SerializeField]
    private GameObject[] floorObj = new GameObject[3];
    [SerializeField]
    private yazirusimove[] yazi=new yazirusimove[2];
    [SerializeField]
    private Sprite[] numimage;
    [SerializeField]
    private Text finishText;
    [SerializeField]
    private SceneMove sceneMove;
    [SerializeField]
    private Sprite popUpAllowImage;
    [SerializeField]
    AudioMixer mixer;
    [SerializeField]
    private AudioClip[] BGMs=new AudioClip[3];

    static public PlaySceneManager SceneManager;
    private Color baseColor;
    private int combo;
    private float elapsedTime = 0;
	private bool BGMPlay = true;
	private int seID=0;
    private int countUpScore;
    private int floorLevel = 0;
    private int defaltScoreFontSize;
    int viewScore = 0;

    static int oneGameTotalScore;

    public enum Condition
    {
        None,
        Reverse,
        Composite
    }

    public static int GetScore()
    {
        return oneGameTotalScore;
    }

    void Awake()
    {
        SceneManager = this;
        Application.targetFrameRate = 60;           //目標FPSを60に設定
        oneGameTotalScore = 0;
        baseColor = directionalLight.color;
        if(scoreText!=null) defaltScoreFontSize = scoreText.fontSize;
        if (BGMs[0] != null) StartCoroutine(BGMChenge(BGMs[0]));
    }
    void Update()
    {
        ScoreView();
        ComboView();
        ReverseTime();
    }

    void ScoreView()
    {
        if (scoreText == null) return;
        scoreText.text = "Score：" + viewScore;
    }
    void ComboView()
    {
        if (comboText == null) return;
        if (combo <= 1) comboText.text = "";
        else comboText.text = combo + "Combo!!";
    }

    public float GetSpeed()
    {
        if (condition == Condition.Composite)
            return (speed  / 100) * Time.deltaTime*60;
        return (speed / 10) * Time.deltaTime*60;
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
            viewScore++;
            ScoreView();
            yield return null;
        }
    }

    IEnumerator ScorePopUpEvent()
    {
        const int m_maxfontSize = 20;
        scoreText.fontSize = defaltScoreFontSize+m_maxfontSize;
        for (; ; )
        {
            if (defaltScoreFontSize >= scoreText.fontSize) break;
            scoreText.fontSize --;
            yield return null;
        }
        scoreText.fontSize = defaltScoreFontSize;
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
		const float m_moveTime = 0.7f;
        const float m_popmoveTime = 0.3f;
        GameObject m_textObj = new GameObject();
		Destroy (m_textObj, m_moveTime+ m_popmoveTime);
        m_textObj.transform.parent = GameObject.Find("Canvas").transform;
        
        TextImageView(score, m_textObj);
		RectTransform m_rect = m_textObj.GetComponent<RectTransform>();

        m_rect.position = scorePos;
        Vector3 m_popMove = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));
        m_rect.DOMove(m_rect.transform.position+ m_popMove, m_popmoveTime).SetEase(Ease.Unset);
        yield return new WaitForSeconds(m_popmoveTime);

        
		m_rect.DOMove(m_scorePos, m_moveTime).SetEase(Ease.InExpo);
		yield return new WaitForSeconds (m_moveTime);
    }

    void TextImageView(int score,GameObject parent)
    {
        var digit = score;
        //要素数0には１桁目の値が格納
        List<int> m_number = new List<int>();
        while (digit != 0)
        {
            score = digit % 10;
            digit = digit / 10;
            m_number.Add(score);
        }

        Image m_image = parent.AddComponent<Image>();
        m_image.sprite = numimage[m_number[0]];

        for (int i = 0; i < m_number.Count; i++)
        {
            //複製
            RectTransform m_scoreimage =new GameObject().AddComponent<RectTransform>();
            m_scoreimage.SetParent(parent.transform, false);
            m_scoreimage.localPosition = new Vector2(
                -m_scoreimage.sizeDelta.x * i,
                0);
            m_scoreimage.gameObject.AddComponent<Image>().sprite = numimage[m_number[i]];
        }
    }

    IEnumerator ScoreUpIenumerator(int getScore, Vector2 scorePos)
    {
        combo++;
        countUpScore += getScore;
        yield return StartCoroutine(ScoreCountMove(getScore, scorePos));
        oneGameTotalScore += getScore;
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

    public void CommandView(int command,int playerID)
    {
        switch (command)
        {
            case 0:
                commandOK[playerID].enabled = true;
                commandAllow[playerID].enabled = false;
                StartCoroutine(CommandPopUp(null, commandOK[playerID]));
                break;
            case 1:
                commandAllow[playerID].enabled = true;
                commandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.forward * 90;
                StartCoroutine(CommandPopUp(commandAllow[playerID]));
                break;
            case 2:
                commandAllow[playerID].enabled = true;
                commandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.forward * 180;
                StartCoroutine(CommandPopUp(commandAllow[playerID]));
                break;
            case 3:
                commandAllow[playerID].enabled = true;
                commandOK[playerID].enabled = false;
                commandAllow[playerID].transform.eulerAngles = Vector3.forward * -90;
                StartCoroutine(CommandPopUp(commandAllow[playerID]));
                break;
            default:
                commandAllow[playerID].enabled = false;
                commandOK[playerID].enabled = false;
                break;
        }
    }

    public void FloorLevelUp()
    {
        StartCoroutine(FloorLevelUpIEnumerator());
    }
    IEnumerator FloorLevelUpIEnumerator()
    {
        const float m_time = 0.5f;
        if(BGMs[floorLevel]!=null) StartCoroutine(BGMChenge(BGMs[floorLevel]));
        if (floorLevel < 2)
        {
            switch (floorLevel)
            {
                case 0:
                    sceneMove.FadeInEvent(new Color(0, 0, 0));
                    yield return new WaitForSeconds(m_time);
                    break;
                case 1:
                    sceneMove.FadeInEvent(new Color(1, 1, 1),1);
                    yield return new WaitForSeconds(m_time);
                    break;
            }

            floorObj[floorLevel].SetActive(false);
            floorLevel++;
            floorObj[floorLevel].SetActive(true);
            sceneMove.FadeOutEvent(new Color(0, 0, 0));
        }
    }

    public void GameClear()
    {
        StartCoroutine(GameClearIEnumerator());
    }

    IEnumerator GameClearIEnumerator()
    {
        const string m_resultName = "Result";
        finishText.enabled=true;
        for (int i = 1; i <= 30; i++) {
            finishText.fontSize = i * 3;
            yield return null;
        }
        yield return new WaitForSeconds(2);
        sceneMove.SceneMoveEvent(m_resultName);
    }

    IEnumerator CommandPopUp(Image commandArrowImage=null,Text commandOKText = null)
    {
        const float m_commandPopUpTime = 0.1f;
        
        Color m_flashColor = Color.white;
        if (commandArrowImage != null)
        {
            const float m_scale = 3;
            Vector3 m_defaultScale = commandArrowImage.transform.localScale;
            Sprite m_arrowSprite = commandArrowImage.sprite;
            commandArrowImage.transform.localScale *= m_scale;
            commandArrowImage.transform.DOScale(m_defaultScale, m_commandPopUpTime);
            commandArrowImage.sprite = popUpAllowImage;
            yield return new WaitForSeconds(m_commandPopUpTime);
            commandArrowImage.sprite = m_arrowSprite;
        }
        else if(commandOKText != null)
        {
            const int m_fontscale = 10;
            commandOKText.fontSize += m_fontscale;
            for(int i = 0; i < 10; i++)
            {
                commandOKText.fontSize--;
                yield return null;
            }
        }
    }

    IEnumerator BGMChenge(AudioClip bgm)
    {
        const string mixerBGMName = "BGM";
        for (float i = 5; i >= 0f; i--)
        {
            mixer.SetFloat(mixerBGMName, -i * 12);
            yield return null;
        }
        BGMaudiosource.Stop();
        BGMaudiosource.clip = bgm;
        BGMaudiosource.Play();
        for (float i = 0; i <= 5f; i++)
        {
            mixer.SetFloat(mixerBGMName, -60 + i * 12);
            yield return null;
        }
    }
}
