using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneManager : MonoBehaviour {

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private bool isCompositeMode = false;
    [SerializeField]
    private Light directionalLight;
    [SerializeField]
    private Color compositeModeLightColor;

    static public PlaySceneManager SceneManager;
    private Color baseColor;
    

    void Awake()
    {
        SceneManager = this;
        Application.targetFrameRate = 60;           //目標FPSを60に設定
        baseColor = directionalLight.color;
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
}
