using nn.hid;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class AttackTest : MonoBehaviour
{
    [SerializeField]
    JoyConControl GetInput;
    Vector3[,] AccelerationKeep = new Vector3[2, 10];
    Vector3[] OldAcceleration = new Vector3[2];
    Vector3[] NowAcceleration = new Vector3[2];
    bool[] SamplingFlag = new bool[2];
    int[] ListCount = new int[2];
    Vector3 DefaultPoint = new Vector3(0, -1, 0);
    Vector3[] max = new Vector3[2];
    Vector3[] min = new Vector3[2];

    bool asd = true;
    [SerializeField]
    RobotControl RoboCon;
    [SerializeField]
    Text txt;

    void Start()
    {
        txt.text = "";
    }
    private NpadState npadState = new NpadState();
    void Update()
    {
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
            Attack(i);
    }

    void Attack(int HandNum)
    {
        const float l_controlValue_z = 3.5f;
        const float l_controlValue_y = 3.5f;
        const float l_controlValue_x = 2.5f;

        Vector3[] Acceleration = new Vector3[2];

        OldAcceleration[HandNum] = NowAcceleration[HandNum];

        if (HandNum == 0)
            NowAcceleration[HandNum] = GetInput.AccelerationGet(Hand.Left) - DefaultPoint;
        if (HandNum == 1)
            NowAcceleration[HandNum] = GetInput.AccelerationGet(Hand.Right) - DefaultPoint;
        txt.text = "" + NowAcceleration[0] + "\n" + "Max" + max[0] + "\n" + "Min" + min[0] + "\n" + "\n" + NowAcceleration[1] + "\n" + "Max" + max[1] + "\n" + "Min" + min[1] + "\n";

        if (asd)
        {
            if (!SamplingFlag[HandNum])
                SamplingFlag[HandNum] = Mathf.Abs(NowAcceleration[HandNum].z) >= l_controlValue_x ||
                    Mathf.Abs(NowAcceleration[HandNum].y) >= l_controlValue_y;

            if (SamplingFlag[HandNum])
            {
                AccelerationKeep[HandNum, ListCount[HandNum]] = NowAcceleration[HandNum];
                ListCount[HandNum]++;

                if (ListCount[HandNum] >= ListCount.Length)
                {
                    Vector3 Vec3 = Vector3.zero;

                    for (int i = 0; i < ListCount[HandNum]; i++)
                        Vec3 += AccelerationKeep[HandNum, i];

                    for (int i = 0; i < ListCount.Length; i++)
                        AccelerationKeep[HandNum, i] = Vector3.zero;

                    Acceleration[HandNum] = Vec3;
                    ListCount[HandNum] = 0;
                    SamplingFlag[HandNum] = false;
                }
            }
            if (max[HandNum].x <= Acceleration[HandNum].x)
                max[HandNum].x = Acceleration[HandNum].x;
            if (max[HandNum].y <= Acceleration[HandNum].y)
                max[HandNum].y = Acceleration[HandNum].y;
            if (max[HandNum].z <= Acceleration[HandNum].z)
                max[HandNum].z = Acceleration[HandNum].z;

            if (min[HandNum].x >= Acceleration[HandNum].x)
                min[HandNum].x = Acceleration[HandNum].x;
            if (min[HandNum].y >= Acceleration[HandNum].y)
                min[HandNum].y = Acceleration[HandNum].y;
            if (min[HandNum].z >= Acceleration[HandNum].z)
                min[HandNum].z = Acceleration[HandNum].z;

            if (Mathf.Abs(Acceleration[HandNum].y) >= l_controlValue_y &&
                Mathf.Abs(Acceleration[HandNum].x) < l_controlValue_z)
            {
                RoboCon.Attack(HandNum, RobotControl.Lane.Center);
                StartCoroutine(DelayTime());
            }
            if (Mathf.Abs(Acceleration[HandNum].z) >= l_controlValue_z)
            {
                if(Acceleration[HandNum].z >= 0)
                {
                    RoboCon.Attack(HandNum, RobotControl.Lane.Right);
                    StartCoroutine(DelayTime());
                }
                else
                {
                    RoboCon.Attack(HandNum, RobotControl.Lane.Left);
                    StartCoroutine(DelayTime());
                }
            }
        }
    }
    IEnumerator DelayTime()
    {
        asd = false;
        yield return new WaitForSeconds(0.2f);
        asd = true;
    }
}
