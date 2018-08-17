using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using nn.hid;

public class SetUp : MonoBehaviour {
    [SerializeField]
    int phase = 0;
    [SerializeField]
    private Text debugText;

    private bool player1ready = false;
    private bool player2ready = false;
    float[] playervalue = new float[2];
    JoyConControl joyConControl;

    

    void Start () {
		
	}
    private NpadState npadState = new NpadState();
    void Update () {
        switch (phase)
        {
            case 0:
                //joyconのボタン入力
                JoyconReadyCheck();
                break;
            case 1:
                //joyconを振る動作
                JoyConShakeReady();
                break;
            case 2:
                //joycon縦持ち
                JoyConStraight();
                break;
            case 3:
                //チュートリアル確認
                TutorialCheck();
                break;
            default:
                break;
        }
	}

    /// <summary>
    /// joyconのボタン入力確認
    /// </summary>
    void JoyconReadyCheck()
    {
        if (ReadyCheck()) return;
        if (!player1ready)
        {
            if (joyConControl.ButtonGet(nn.hid.NpadButton.ZL))
            {
                player1ready = true;
            }
        }
        if (!player2ready)
        {
            if (joyConControl.ButtonGet(nn.hid.NpadButton.ZR))
            {
                player2ready = true;
            }
        }
    }

    /// <summary>
    /// joyconを振る動作
    /// </summary>
    void JoyConShakeReady()
    {
        float m_readyShakePower = 25;

        if (ReadyCheck()) return;
        ShakePowerCheck();
        if (!player1ready)
        {
            if (playervalue[0] > m_readyShakePower)
            {
                player1ready = true;
            }
        }
        if (!player2ready)
        {
            if (playervalue[1] > m_readyShakePower)
            {
                player2ready = true;
            }
        }
    }

    /// <summary>
    /// JoyConを縦持ちする処理
    /// </summary>
    void JoyConStraight()
    {
        Debug.Log("ここで縦持ちの処理");
        player1ready = true;
        player2ready = true;
        if (ReadyCheck()) return;
    }

    /// <summary>
    /// 両プレイヤー準備を完了したか確認する
    /// </summary>
    /// <returns>両プレイヤー完了していた場合trueを返し両方の準備状態をfalseにする</returns>
    bool ReadyCheck()
    {
        if (player1ready && player2ready)
        {
            player1ready = false;
            player2ready = false;
            playervalue[0] = 0;
            playervalue[1] = 0;
            phase++;
            return true;
        }
        return false;
    }

    void DebugStatusPrint()
    {
        debugText.text = "phase:" + phase + "　P1Ready:" + player2ready + "　P1Ready:" + player2ready;
    }

    void TutorialCheck()
    {

    }

    void ShakePowerCheck()
    {
        playervalue[0] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).x);
        playervalue[0] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).z);
        playervalue[1] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).x);
        playervalue[1] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).z);
    }
}
