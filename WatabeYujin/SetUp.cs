using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using nn.hid;

public class SetUp : MonoBehaviour {
    [SerializeField]
    JoyConControl joyConControl;
    [SerializeField]
    int phase = 0;
    [SerializeField]
    private Text indexText;
    [SerializeField]
    private Text debugText;
    [SerializeField]
    private Text[] readyTexts = new Text[2];
    [SerializeField]
    private Image[] indicatorImage = new Image[2];
    [SerializeField]
    private RectTransform[] selectImage = new RectTransform[2];
    [SerializeField]
    private GameObject tutorialMenu;
    [SerializeField]
    private SceneMove sceneMove;
    [SerializeField]
    private GameObject maingame;

    private bool player1ready = false;
    private bool player2ready = false;
    float[] playervalue = new float[2];
    Vector3[] debugvalue = new Vector3[2];
    private int startIndexFontSize;
    private bool isRaadyRun = true;
    bool[] tutorialSkip = new bool[2];

    void Start () {
        startIndexFontSize = indexText.fontSize;
	}
    private NpadState npadState = new NpadState();
    void Update () {
        DebugPlayerValueUP();
        DebugStatusPrint();
        if (!isRaadyRun) return;
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
        indexText.text = "ZRボタン・ZLボタンをおしてクダサイ";
        if (!player1ready)
        {
            if (joyConControl.ButtonGet(nn.hid.NpadButton.ZL))
            {
                player1ready = true;
                Readyindex(0, true);
            }
        }
        if (!player2ready)
        {
            if (joyConControl.ButtonGet(nn.hid.NpadButton.ZR))
            {
                player2ready = true;
                Readyindex(1, true);
            }
        }
    }

    /// <summary>
    /// joyconを振る動作
    /// </summary>
    void JoyConShakeReady()
    {
        const float m_readyShakePower = 100;
        IndicatorView(m_readyShakePower);
        if (ReadyCheck()) return;
        indexText.text = "Joy-Con(c)をふってクダサイ";
        ShakePowerCheck();
        if (!player1ready)
        {
            if (playervalue[0] > m_readyShakePower)
            {
                player1ready = true;
                Readyindex(0, true);
            }
        }
        if (!player2ready)
        {
            if (playervalue[1] > m_readyShakePower)
            {
                player2ready = true;
                Readyindex(1, true);
            }
        }
    }

    /// <summary>
    /// JoyConを縦持ちする処理
    /// </summary>
    void JoyConStraight()
    {
        if (ReadyCheck()) return;
        indexText.text = "Joy-Con(c)をタテににぎってクダサイ";
        StraightTimeCheck();
    }

    IEnumerator PrintSuccess()
    {
        player1ready = false;
        player2ready = false;
        phase++;
        indexText.text = "OK！！！";
        indexText.fontSize += 10;
        yield return StartCoroutine(TextPopUp(indexText));
        StartCoroutine(IndicatorClose());
        yield return new WaitForSeconds(2f);
        while (indexText.fontSize>=0)
        {
            indexText.fontSize--;
            yield return null;
        }
        indexText.fontSize = 0;
        playervalue[0] = 0;
        playervalue[1] = 0;
        Readyindex(0, false);
        Readyindex(1, false);
        isRaadyRun = true;
        while (indexText.fontSize <= startIndexFontSize)
        {
            indexText.fontSize++;
            yield return null;
        }
        if (phase == 3) tutorialMenu.SetActive(true);
    }

    /// <summary>
    /// 両プレイヤー準備を完了したか確認する
    /// </summary>
    /// <returns>両プレイヤー完了していた場合trueを返し次のフェイズへ移行する</returns>
    bool ReadyCheck()
    {
        if (player1ready && player2ready)
        {
            isRaadyRun = false;
            StartCoroutine(PrintSuccess());
            return true;
        }
        return false;
    }

    void Readyindex(int playerID, bool isReady)
    {
        if (!isReady)
        {
            readyTexts[playerID].text = "";
        }
        else
        {
            readyTexts[playerID].text = "Ready!";
            StartCoroutine(TextPopUp(readyTexts[playerID]));
        }
        
    }

    IEnumerator TextPopUp(Text text)
    {
        text.fontSize += 10;
        for (int i = 0; i < 10; i++)
        {
            text.fontSize--;
            yield return null;
        }
    }

    void DebugStatusPrint()
    {
        debugText.text = "phase:" + phase + "　playervalue[0]:" + playervalue[0] + "　playervalue[1]:" + "\n playervalue[0]" + debugvalue[0] + "playervalue[1]" + debugvalue[1];
    }

    void TutorialCheck()
    {
        indexText.text = "チュートリアルをプレイしマスカ？";
        if (joyConControl.ButtonGet(nn.hid.NpadButton.ZL,Style.Down))
        {
            player1ready = !player1ready;
            Readyindex(0, !player1ready);
        }
        if (joyConControl.ButtonGet(nn.hid.NpadButton.ZR,Style.Down))
        {
            player2ready = !player2ready;
            Readyindex(1, !player2ready);
        }
        if (player1ready && player2ready)
        {
            if (tutorialSkip[0] != tutorialSkip[1]) return;
            if (tutorialSkip[0])
                StartCoroutine(MainGameStart());
            else
                TutorialStart();
        }
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftArrow))
            TutorialSelected(1, false);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            TutorialSelected(1, true);
        if (Input.GetKeyDown(KeyCode.A))
            TutorialSelected(0, false);
        else if (Input.GetKeyDown(KeyCode.D))
            TutorialSelected(0, true);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            player1ready = !player1ready;
            Readyindex(0, player1ready);
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            player2ready = !player2ready;
            Readyindex(1, player2ready);
        }
#endif
    }

    public void TutorialSelected(int playerID,bool isSkip)
    {
        float[] m_skipPosX = new float[2] { 207, 193 };
        float[] m_notSkipPosX = new float[2] { -177, -193 };
        const float m_posY = 0;
        if (phase != 3) return;
        if (playerID == 0 && player1ready==true) return;
        if (playerID == 1 && player2ready == true) return;
        switch (isSkip)
        {
            case true:
                selectImage[playerID].localPosition = new Vector2(m_skipPosX[playerID], m_posY);
                break;
            case false:
                selectImage[playerID].localPosition = new Vector2(m_notSkipPosX[playerID], m_posY);
                break;
        }
        tutorialSkip[playerID] = isSkip;
    }

    void ShakePowerCheck()
    {
        if(Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).x)>=2)
            playervalue[0] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).x);
        if (Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).z) >= 2)
            playervalue[0] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Left).z);
        if (Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).x) >= 2)
            playervalue[1] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).x);
        if (Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).z) >= 2)
            playervalue[1] += Mathf.Abs(joyConControl.AccelerationGet(Hand.Right).z);
    }

    bool StraightCheck(int playerID)
    {
        const float m_allowableValue = 0.3f;
        Vector3 m_checkValue;
        if (playerID == 0)
        {
            m_checkValue = joyConControl.AccelerationGet(Hand.Left);
            m_checkValue += Vector3.up;
            debugvalue[0] = m_checkValue;
        }
        else
        {
            m_checkValue = joyConControl.AccelerationGet(Hand.Right);
            m_checkValue += Vector3.up;
            debugvalue[1] = m_checkValue;
        }
        if (Mathf.Abs(m_checkValue.x) > m_allowableValue) return false;
        if (Mathf.Abs(m_checkValue.y) > m_allowableValue) return false;
        if (Mathf.Abs(m_checkValue.z) > m_allowableValue) return false;
        return true;
    }

    void StraightTimeCheck()
    {
        const float m_straightTime = 3;
        IndicatorView(m_straightTime);
        for (int i = 0; i < 2; i++)
        {
            if (StraightCheck(i))
                playervalue[i] += Time.deltaTime;
        }
        if (!player1ready && playervalue[0] >= m_straightTime)
        {
            Readyindex(0, true);
            player1ready = true;
        }
        if (!player2ready && playervalue[1] >= m_straightTime)
        {
            Readyindex(1, true);
            player2ready = true;
        }
    }
    IEnumerator IndicatorClose()
    {
        for (float i = 0; i <= 10; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                indicatorImage[j].fillAmount = 1-i/10f;
                yield return null;
            }
        }
    }
    void IndicatorView(float max)
    {
        for(int i = 0; i < 2; i++)
        {
            indicatorImage[i].fillAmount = playervalue[i] / max;
        }
    }
    void DebugPlayerValueUP()
    {
        if (Input.GetKey(KeyCode.LeftShift))
            playervalue[0] += Time.deltaTime;
        if (Input.GetKey(KeyCode.RightShift))
            playervalue[1] += Time.deltaTime;
    }

    IEnumerator MainGameStart()
    {
        phase++;
        const float m_fadeTime = 1f;
        sceneMove.FadeInEvent();
        yield return new WaitForSeconds(m_fadeTime);
        
        maingame.SetActive(true);
        gameObject.SetActive(false);
    }
    void TutorialStart()
    {

    }
}
