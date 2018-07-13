using nn.hid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AccelerationInput : MonoBehaviour
{
    [SerializeField]
    JoyConControl GetInput;
    /// <summary>accelerationのサンプルを入れる配列</summary>
    Vector3[,] accelerationKeep = new Vector3[2, 10];
    /// <summary>現在のAcceleration</summary>
    Vector3[] nowAcceleration = new Vector3[2];
    /// <summary>サンプリング開始用Flag</summary>
    bool[] samplingFlag = new bool[2];
    /// <summary>配列に入れるためのインデックスの代わり</summary>
    int[] listCount = new int[2];
    /// <summary>指定したコントローラの持ち方の加速度センサーのデフォルト</summary>
    Vector3 defaultPoint = new Vector3(0, -1, 0);
    Vector3[] max = new Vector3[2];
    Vector3[] min = new Vector3[2];
    /// <summary>ディレイ用Flag</summary>
    bool delayFlag = true;
    [SerializeField]
    RobotControl roboCon;
    [SerializeField]
    Text txt;
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
        txt.text = "";
    }
    private NpadState npadState = new NpadState();
    void Update()
    {
        ControlvalueButton();
        if (GetInput.ButtonGet(NpadButton.ZL, Style.Down) || GetInput.ButtonGet(NpadButton.ZR, Style.Down))
        {
            txt.text = "";
            max[0] = Vector3.zero;
            max[1] = Vector3.zero;
            min[1] = Vector3.zero;
            min[0] = Vector3.zero;
        }
        if (GetInput.ButtonGet(NpadButton.Plus) || GetInput.ButtonGet(NpadButton.Minus))
        {
            SceneManager.LoadScene("test");
        }
        for (int i = 0; i <= 1; i++)
            SwingCheck(i);
    }

    void SwingCheck(int handNum)
    {

        const float m_shakeStart_z = 3.5f;
        const float m_shakeStart_y = 3.5f;
        Vector3[] m_acceleration = new Vector3[2];

        if (handNum == 0)
        {
            nowAcceleration[handNum] = GetInput.AccelerationGet(Hand.Left) - defaultPoint;
            nowAcceleration[handNum].z *= -1;
        }
        if (handNum == 1)
            nowAcceleration[handNum] = GetInput.AccelerationGet(Hand.Right) - defaultPoint;

        if (delayFlag)
        {
            if (!samplingFlag[handNum])
                samplingFlag[handNum] = Mathf.Abs(nowAcceleration[handNum].z) >= m_controlValue_z[handNum] ||
                    Mathf.Abs(nowAcceleration[handNum].y) >= m_controlValue_y[handNum];

            if (samplingFlag[handNum])
            {
                accelerationKeep[handNum, listCount[handNum]] = nowAcceleration[handNum];
                listCount[handNum]++;

                if (listCount[handNum] >= listCount.Length)
                {
                    Vector3 Vec3 = Vector3.zero;

                    for (int i = 0; i < 10; i++)
                        Vec3 += accelerationKeep[handNum, i];

                    for (int i = 0; i < 10; i++)
                        accelerationKeep[handNum, i] = Vector3.zero;

                    m_acceleration[handNum] = Vec3;
                    listCount[handNum] = 0;
                    samplingFlag[handNum] = false;
                }
            }
            if (max[handNum].x <= m_acceleration[handNum].x)
                max[handNum].x = m_acceleration[handNum].x;
            if (max[handNum].y <= m_acceleration[handNum].y)
                max[handNum].y = m_acceleration[handNum].y;
            if (max[handNum].z <= m_acceleration[handNum].z)
                max[handNum].z = m_acceleration[handNum].z;

            if (min[handNum].x >= m_acceleration[handNum].x)
                min[handNum].x = m_acceleration[handNum].x;
            if (min[handNum].y >= m_acceleration[handNum].y)
                min[handNum].y = m_acceleration[handNum].y;
            if (min[handNum].z >= m_acceleration[handNum].z)
                min[handNum].z = m_acceleration[handNum].z;

            //if (Mathf.Abs(m_acceleration[handNum].y) >= m_controlValue_y &&
            //    Mathf.Abs(m_acceleration[handNum].z) < m_controlValue_z)
            if (Mathf.Abs(m_acceleration[handNum].y) >= m_controlValue_y[handNum] &&
                Mathf.Abs(m_acceleration[handNum].z) < y_shake_range[handNum])
            {
                //縦の判定
                roboCon.Attack(handNum, RobotControl.Lane.Center);
                StartCoroutine(DelayTime());
            }
            //else if (Mathf.Abs(m_acceleration[handNum].z) >= m_controlValue_z)
            else if (Mathf.Abs(m_acceleration[handNum].z) >= m_controlValue_z[handNum])
            {
                if (m_acceleration[handNum].z >= 0)
                {
                    roboCon.Attack(handNum, RobotControl.Lane.Right);
                    StartCoroutine(DelayTime());
                }
                else
                {
                    roboCon.Attack(handNum, RobotControl.Lane.Left);
                    StartCoroutine(DelayTime());
                }
            }
        }
    }
    IEnumerator DelayTime()
    {
        delayFlag = false;
        yield return new WaitForSeconds(0.2f);
        delayFlag = true;
    }

    void ControlvalueButton()
    {
        controlValueText[0].text = "Y：" + m_controlValue_y[0] + "\n" + "Z：" + m_controlValue_z[0]+ "\n" + "縦許容：" + y_shake_range[0];
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
