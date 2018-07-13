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
<<<<<<< HEAD
    [SerializeField]
    private yazirusimove[] yazi=new yazirusimove[2];
=======
>>>>>>> a7ac216852c9f9d62f3f202682bfe8676e393025

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
<<<<<<< HEAD
        if (combo <= 1) comboText.text = "";
        else comboText.text = combo + "Combo!!";
=======
        if (combo <= 1) return;
<<<<<<< HEAD
        comboText.text = combo + "Combo!!";
=======
        comboText.text = comboText + "Combo!!";
>>>>>>> origin/master
>>>>>>> a7ac216852c9f9d62f3f202682bfe8676e393025
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
<<<<<<< HEAD

	public void BGMisPlay(bool value){
		if (value)
			BGMaudiosource.Play ();
		else
			BGMaudiosource.Pause ();
=======

	public void BGMisPlay(bool value){
		if (value)
			BGMaudiosource.Play ();
		else
			BGMaudiosource.Pause ();
	}
	public void SEPlay(AudioClip se){
		SEaudiosource [seID].clip = se;
		SEaudiosource [seID].Play ();
<<<<<<< HEAD
=======

>>>>>>> origin/master
	}
	public void SEPlay(AudioClip se){
		SEaudiosource [seID].clip = se;
		SEaudiosource [seID].Play ();
>>>>>>> a7ac216852c9f9d62f3f202682bfe8676e393025
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
