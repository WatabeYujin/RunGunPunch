using nn.hid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AccelerationInputSet : MonoBehaviour
{
    public enum Line
    { Center = 0, Left = 1, Right = 2, }
    public enum ControllerGrip
    { Left = 1, Right = 2, }
    [SerializeField]
    JoyConControl GetInput;
    /// <summary>accelerationのサンプルを入れる配列</summary>
    Vector3[,] accelerationKeep = new Vector3[2, 20];
    /// <summary>現在のAcceleration</summary>
    Vector3[] nowAcceleration = new Vector3[2];
    /// <summary>サンプリング開始用Flag</summary>
    bool[] samplingFlag = new bool[2];
    /// <summary>配列に入れるためのインデックスの代わり</summary>
    int[] listCount = new int[2];
    /// <summary>指定したコントローラの持ち方の加速度センサーのデフォルト</summary>
    Vector3 defaultPoint = new Vector3(0, -1, 0);
    float[] side = new float[2];
    /// <summary>振った方向ごとの加速度の合計を格納するfloatの配列（0=左 1=真ん中 2=右）</summary>
    float[,] mix = new float[2, 3];
    /// <summary>配列のindex</summary>
    int[] index = new int[2];
    bool[] check = { false, false };
    /// <summary>ディレイ用Flag</summary>
    bool[] delayFlag = { false, false };
    int listSize = 2;
    //ログ用の変数****************************************************
    Vector3[,] log = new Vector3[2, 5];
    RobotControl.Lane[] Lane;
    RobotControl.Lane[,] logLane = new RobotControl.Lane[2, 5];
    Vector3[,] origin = new Vector3[2, 20];
    Vector3[,] Sample = new Vector3[2, 20];
    [SerializeField]
    Text txt;
    [SerializeField]
    Text txt1;
    [SerializeField]
    Text txt2;
    //********************************************************
    [SerializeField]
    RobotControl roboCon;

    [SerializeField]
    Text[] controlValueText = new Text[2];

    [SerializeField]
    float[] m_controlValue_z = new float[2] { 3.5f, 3.5f };
    [SerializeField]
    float[] m_controlValue_y = new float[2] { 3.5f, 3.5f };
    [SerializeField]
    float[] y_shake_range = new float[2] { 3.5f, 3.5f };

    void Start()
    {
        Debug.Log(PlayerPrefs.GetInt(saveKey));
        LogScore = PlayerPrefs.GetInt(saveKey);

        txt.text =
            "Left\n" + log[0, 0] + "\n" + log[0, 1] + "\n" + log[0, 2] + "\n" + log[0, 3] + "\n" + log[0, 4] + "\n\n" +
            "Right\n" + log[1, 0] + "\n" + log[1, 1] + "\n" + log[1, 2] + "\n" + log[1, 3] + "\n" + log[1, 4] + "\n";
        txt1.text = "OriginLeft\n" +
                          origin[0, 0] + "\n" + origin[0, 1] + "\n" + origin[0, 2] + "\n" + origin[0, 3] + "\n" + origin[0, 4] + "\n\n" +
                       "originRight\n" +
                          origin[1, 0] + "\n" + origin[1, 1] + "\n" + origin[1, 2] + "\n" + origin[1, 3] + "\n" + origin[1, 4];
        txt2.text = "SampleLeft\n" +
                          origin[0, 0] + "\n" + origin[0, 1] + "\n" + origin[0, 2] + "\n" + origin[0, 3] + "\n" + origin[0, 4] + "\n\n" +
                    "SampleRight\n" +
                          origin[1, 0] + "\n" + origin[1, 1] + "\n" + origin[1, 2] + "\n" + origin[1, 3] + "\n" + origin[1, 4];
    }
    private NpadState npadState = new NpadState();
    void Update()
    {
        testText.text = "" + LogScore;
        testText1.text = "" + Score;
        if (GetInput.ButtonGet(NpadButton.R, Style.Down) || GetInput.ButtonGet(NpadButton.L, Style.Down))
        { test(); }
          ControlvalueButton();
        if (GetInput.ButtonGet(NpadButton.ZL, Style.Down) || GetInput.ButtonGet(NpadButton.ZR, Style.Down))
        {
            txt.text =
            "Left\n" +
            log[0, 0] + logLane[0, 0] + "\n" + log[0, 1] + logLane[0, 1] + "\n" + log[0, 2] + logLane[0, 2] + "\n" +
            log[0, 3] + logLane[0, 3] + "\n" + log[0, 4] + logLane[0, 4] + "\n" +
            "\nRight\n" +
            log[1, 0] + logLane[1, 0] + "\n" + log[1, 1] + logLane[1, 1] + "\n" + log[1, 2] + logLane[1, 2] + "\n" +
            log[1, 3] + logLane[1, 3] + "\n" + log[1, 4] + logLane[1, 4] + "\n";
        }
        if (GetInput.ButtonGet(NpadButton.Plus) || GetInput.ButtonGet(NpadButton.Minus))
        {
            SceneManager.LoadScene("test");
        }
        for (int i = 0; i <= 1; i++)
            SwingCheck(i);
    }
    int LogScore = 0;
    int Score = 0;
    string saveKey = "save";

    public Text testText;
    public Text testText1;

    void test()
    {
        LogScore++;
        Score++;
        PlayerPrefs.SetInt(saveKey, LogScore);
        PlayerPrefs.Save();
    }

    
    void SwingCheck(int handNum)
    {
        Vector3[] m_acceleration = new Vector3[2];

        if (handNum == 0)
        {
            nowAcceleration[handNum] = GetInput.AccelerationGet(Hand.Left);
            nowAcceleration[handNum].z *= -1;
        }

        if (handNum == 1)
            nowAcceleration[handNum] = GetInput.AccelerationGet(Hand.Right);

        if (!delayFlag[handNum])
        {
            if (!samplingFlag[handNum])
                samplingFlag[handNum] =
                    Mathf.Abs(nowAcceleration[handNum].z) >= m_controlValue_z[handNum] ||
                    Mathf.Abs(nowAcceleration[handNum].y) >= m_controlValue_y[handNum];

            if (samplingFlag[handNum])
            {
                accelerationKeep[handNum, listCount[handNum]] = nowAcceleration[handNum];
                listCount[handNum]++;
                txt1.text = "OriginLeft\n" +
                          accelerationKeep[0, 0] + "\n" + accelerationKeep[0, 1] + "\n" + accelerationKeep[0, 2] + "\n" + accelerationKeep[0, 3] + "\n" + accelerationKeep[0, 4] + "\n" +
                      "\nOriginRight\n" +
                          accelerationKeep[1, 0] + "\n" + accelerationKeep[1, 1] + "\n" + accelerationKeep[1, 2] + "\n" + accelerationKeep[1, 3] + "\n" + accelerationKeep[1, 4];
                if (listCount[handNum] >= listSize)
                {
                    for (int i = 0; i < listSize; i++)
                    {
                        

                        if (accelerationKeep[handNum, i].z <= 0)
                            mix[handNum, (int)Line.Left] += accelerationKeep[handNum, i].z;
                        if (accelerationKeep[handNum, i].z >= 0)
                            mix[handNum, (int)Line.Right] += accelerationKeep[handNum, i].z;

                        if (//Mathf.Abs(accelerationKeep[handNum, i].z) < y_shake_range[handNum] &&
                             Mathf.Abs(accelerationKeep[handNum, i].y) >= m_controlValue_y[handNum])
                            mix[handNum, (int)Line.Center] += accelerationKeep[handNum, i].y;

                        Sample[handNum, i] = new Vector3(mix[handNum, (int)Line.Left], mix[handNum, (int)Line.Center], mix[handNum, (int)Line.Right]);
                        txt2.text = "SampleLeft\n" +
                           Sample[0, 0] + "\n" + Sample[0, 1] + "\n" + Sample[0, 2] + "\n" + Sample[0, 3] + "\n" + Sample[0, 4] +
                       "\nSampleRight\n" +
                           Sample[1, 0] + "\n" + Sample[1, 1] + "\n" + Sample[1, 2] + "\n" + Sample[1, 3] + "\n" + Sample[1, 4];
                    }

                    Support(handNum,mix);

                    side[handNum] =
                        Mathf.Abs(mix[handNum, (int)Line.Left]) > Mathf.Abs(mix[handNum, (int)Line.Right]) ?
                        mix[handNum, (int)Line.Left] : mix[handNum, (int)Line.Right];

                    for (int i = 0; i < listSize; i++)
                        accelerationKeep[handNum, i] = Vector3.zero;
                    listCount[handNum] = 0;
                    samplingFlag[handNum] = false;
                    check[handNum] = true;
                }
            }
            if (check[handNum])
            {
                if (Mathf.Abs(side[handNum]) > Mathf.Abs(mix[handNum, (int)Line.Center]))
                {
                    if (side[handNum] >= 0)
                    {
                        roboCon.Attack(handNum, RobotControl.Lane.Right);
                        StartCoroutine(DelayTime(handNum, RobotControl.Lane.Right));
                    }
                    else
                    {
                        roboCon.Attack(handNum, RobotControl.Lane.Left);
                        StartCoroutine(DelayTime(handNum, RobotControl.Lane.Left));
                    }
                }
                else
                {
                    //if (Mathf.Abs(side[handNum]) - Mathf.Abs(mix[handNum, (int)Line.Center]) > 0.5f)
                    if (Mathf.Abs(side[handNum]) > 6f)
                    {
                        if (side[handNum] >= 0)
                        {
                            roboCon.Attack(handNum, RobotControl.Lane.Right);
                            StartCoroutine(DelayTime(handNum, RobotControl.Lane.Right));
                        }
                        else
                        {
                            roboCon.Attack(handNum, RobotControl.Lane.Left);
                            StartCoroutine(DelayTime(handNum, RobotControl.Lane.Left));
                        }
                    }
                    else
                    {
                        //縦の判定
                        roboCon.Attack(handNum, RobotControl.Lane.Center);
                        StartCoroutine(DelayTime(handNum, RobotControl.Lane.Center));
                    }
                }
                check[handNum] = false;
            }
        }
    }

    IEnumerator DelayTime(int handNum, RobotControl.Lane Lane)
    {
        delayFlag[handNum] = true;
        yield return new WaitForSeconds(0.3f);
        log[handNum, index[handNum]] = new Vector3(mix[handNum, (int)Line.Left], mix[handNum, (int)Line.Center], mix[handNum, (int)Line.Right]);
        logLane[handNum, index[handNum]] = Lane;
        if (index[handNum] != 4)
            index[handNum]++;
        else
            index[handNum] = 0;
        txt.text =
            "Left\n" +
            log[0, 0] + logLane[0, 0] + "\n" + log[0, 1] + logLane[0, 1] + "\n" + log[0, 2] + logLane[0, 2] + "\n" +
            log[0, 3] + logLane[0, 3] + "\n" + log[0, 4] + logLane[0, 4] + "\n" +
            "\nRight\n" +
             log[1, 0] + logLane[1, 0] + "\n" + log[1, 1] + logLane[1, 1] + "\n" + log[1, 2] + logLane[1, 2] + "\n" +
             log[1, 3] + logLane[1, 3] + "\n" + log[1, 4] + logLane[1, 4] + "\n";

        for (int i = 0; i < 3; i++)
            mix[handNum, i] = 0;
        delayFlag[handNum] = false;
    }
    void Support(int handNum,float[,] mix)
    {
        if(false)
        {
            switch(false/*gripHand[handNum]*/)
            {
                //case /*Left*///mix[handNum,(int)Line.Right]*1.02;break;
                //case /*Right*///mix[handNum,(int)Line.Left]*1.02;break;
                default: break;
            }
        }

        
    }
    void ControlvalueButton()
    {
        controlValueText[0].text = "Y：" + m_controlValue_y[0] + "\n" + "Z：" + m_controlValue_z[0] + "\n" + "縦許容：" + y_shake_range[0];
        controlValueText[1].text = "Y：" + m_controlValue_y[1] + "\n" + "Z：" + m_controlValue_z[1] + "\n" + "縦許容：" + y_shake_range[1];
        //横軸調整
        if (GetInput.ButtonGet(NpadButton.A, Style.Down))
            m_controlValue_z[1] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.Y, Style.Down))
            m_controlValue_z[1] -= 0.2f;
        if (GetInput.ButtonGet(NpadButton.Right, Style.Down))
            m_controlValue_z[0] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.Left, Style.Down))
            m_controlValue_z[0] -= 0.2f;
        //縦軸調整
        if (GetInput.ButtonGet(NpadButton.X, Style.Down))
            m_controlValue_y[1] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.B, Style.Down))
            m_controlValue_y[1] -= 0.2f;
        if (GetInput.ButtonGet(NpadButton.Up, Style.Down))
            m_controlValue_y[0] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.Down, Style.Down))
            m_controlValue_y[0] -= 0.2f;
        //縦振り時の横移動許容範囲
        if (GetInput.ButtonGet(NpadButton.LeftSL, Style.Down))
            y_shake_range[0] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.LeftSR, Style.Down))
            y_shake_range[0] -= 0.2f;
        if (GetInput.ButtonGet(NpadButton.RightSL, Style.Down))
            y_shake_range[1] += 0.2f;
        if (GetInput.ButtonGet(NpadButton.RightSR, Style.Down))
            y_shake_range[1] -= 0.2f;

    }
}
