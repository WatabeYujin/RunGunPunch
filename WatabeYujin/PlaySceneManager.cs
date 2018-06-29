using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

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

    static public PlaySceneManager SceneManager;
    private Color baseColor;
    private int score;
    private int combo;
    private float elapsedTime = 0;
	private bool BGMPlay = true;
	private int seID=0;
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
        if (combo <= 1) return;
        comboText.text = comboText + "Combo!!";
    }

    public float GetSpeed()
    {
        return speed / 10;
    }
    void ReverseTime()
    {
        if (GetSetNowCondition == Condition.Reverse)
        {
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

    public void ScoreUP(int getScore)
    {
        combo++;
        score += getScore;
    }

    public void ComboStop()
    {
        combo = 0;
    }

	public void BGMisPlay(bool value){
		if (value)
			BGMaudiosource.Play ();
		else
			BGMaudiosource.Stop ();
	}
	public void SEPlay(AudioClip se){
		SEaudiosource [seID].clip = se;
		SEaudiosource [seID].Play ();

	}
}
