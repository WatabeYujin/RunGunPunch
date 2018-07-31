﻿using System.Collections;
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
    [SerializeField]
    private AudioClip compositeSE;
    [SerializeField]
    private Text energyComandText;
    [SerializeField]
    private Text overComandText;
    [SerializeField]
    private yazirusimove[] yazi=new yazirusimove[2];


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
            return speed / 50;
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

    public void ScoreUP(int getScore)
    {
        combo++;
        countUpScore += getScore;
        StartCoroutine(ScoreCountUp());
        StartCoroutine(ScorePopUpEvent());
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
        string m_viewText = "";
        switch (comand)
        {
            case 0:
                m_viewText = "OK！";
                break;
            case 1:
                m_viewText = "←";
                break;
            case 2:
                m_viewText = "↓";
                break;
            case 3:
                m_viewText = "→";
                break;
            default:
                m_viewText = "";
                break;
        }
        if (playerID == 0)
        {
            energyComandText.text = m_viewText;
        }
        else
        {
            overComandText.text = m_viewText;
        }
    }


}
