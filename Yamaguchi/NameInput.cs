using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NameInput : MonoBehaviour {

    [SerializeField]
    private GameObject startButton;
    private int playerNum = 0;
    [SerializeField]
    private Text[] nameTexts = new Text[2];
    private string[] nameStrs = { "", "" };

	// Use this for initialization
	void Start () {
        StartButton();
        for(int i = 0; i < 2; i++)
        {
            nameTexts[i].text = nameStrs[i];
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// 文字入力処理
    /// </summary>
    /// <param name="m_text">ボタンの子オブジェクトのテキスト</param>
    public void NameButton(Text m_text)
    {
        if(playerNum < 2 &&  nameStrs[playerNum].Length < 5)
        {
            nameStrs[playerNum] += m_text.text;
            nameTexts[playerNum].text = nameStrs[playerNum];
        }
    }
    /// <summary>
    /// 文字を一文字削除
    /// </summary>
    public void Back()
    {
        int length = nameStrs[playerNum].Length;
        Debug.Log(length);
        if (length > 0 && playerNum < 2)
        {
            nameStrs[playerNum] = nameStrs[playerNum].Remove(length-1);
            nameTexts[playerNum].text = nameStrs[playerNum];
        }
    }
    /// <summary>
    /// 決定
    /// </summary>
    public void Decision()
    {
        if(nameStrs[playerNum].Length < 1)
        {
            return;
        }
        playerNum++;
        StartButton();
        if (playerNum >= 2)
        {
            Debug.Log("OK");
        }
    }
    /// <summary>
    /// Aボタンにカーソルを合わせる
    /// </summary>
    private void StartButton()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    

    

 
}
