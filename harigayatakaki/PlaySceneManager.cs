using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlaySceneManager : MonoBehaviour {

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private bool isCompositeMode = false;
    [SerializeField]
    private Light directionalLight;
    [SerializeField]
    private Color compositeModeLightColor;
	[SerializeField]
	private Text scoreText;
	[SerializeField]
	private Text comboText;

    static public PlaySceneManager SceneManager;
    private Color baseColor;
	private int score;
	private int combo;

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
		comboText.text = combo + "Combo!!";
	}

    public float GetSpeed()
    {
        return speed / 10;
    }
    public bool GetSetCompositeMode
    {
        get{
            return isCompositeMode;
        }
        set{
            if (isCompositeMode == value) return;
            isCompositeMode = value;
            LightColorChange(value);
        }
    }

    void LightColorChange(bool isComposite)
    {
        if (isComposite) directionalLight.color = compositeModeLightColor;
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
