using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviour {

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private Light directionalLight;
    [SerializeField]
    private Color compositeModeLightColor;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Text comboText;
    [SerializeField]
    private Condition condition;

    static public PlaySceneManager SceneManager;
    private Color baseColor;
	private int score;
	private int combo;
    

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
	void Update(){
		ScoreView ();
		ComboView ();
	}

	void ScoreView(){
		scoreText.text = "Score："+score;
	}
	void ComboView(){
		if (combo <= 1) return;
		comboText.text = comboText + "Combo!!";
	}

    public float GetSpeed()
    {
        return speed / 10;
    }
    public Condition GetSetNowCondition
    {
        get{
            return condition;
        }
        set{
            if (condition == value) return;
            condition = value;
            LightColorChange();
        }
    }

    void LightColorChange()
    {
        if (condition==Condition.Composite) directionalLight.color = compositeModeLightColor;
        else directionalLight.color = baseColor;
    }

	public void ScoreUP(int getScore){
		combo++;
		score += getScore;
	}

	public void ComboStop(){
		combo = 0;
	}
}
