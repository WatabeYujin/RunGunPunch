using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySceneManager : MonoBehaviour {

    [SerializeField]
    private float speed = 1;
    [SerializeField]
    private bool isCompositeMode = false;

    static public PlaySceneManager SceneManager;
    

    void Awake()
    {
        SceneManager = this;
        Application.targetFrameRate = 60;           //目標FPSを60に設定
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
            isCompositeMode = value;
        }
    }
}
