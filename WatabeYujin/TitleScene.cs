using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour {
    [SerializeField]
    private JoyConControl joycon;
    [SerializeField]
    private SceneMove sceneMove;
    [SerializeField]
    private string sceneName;
    bool ispush = false;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (ispush) return;
		if(joycon.ButtonGet(nn.hid.NpadButton.ZL)|| joycon.ButtonGet(nn.hid.NpadButton.ZR)||
            Input.GetKeyDown(KeyCode.Space))
        {
            ispush = true;
            sceneMove.SceneMoveEvent(sceneName);
        }
	}
}
