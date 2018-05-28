using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMove : MonoBehaviour {
    [SerializeField]
    PlaySceneManager sceneManager;  //あらかじめおいているオブジェクトなのでシリアライズ化
    [SerializeField]
    Renderer thisRenderer;
	
	void Update () {
        thisRenderer.material.SetFloat("_FloorSpeed", sceneManager.GetSpeed());
    }
}
