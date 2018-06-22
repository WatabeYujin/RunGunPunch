﻿using nn.hid;
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
    Vector3 max = Vector3.zero;
    Vector3 min = Vector3.zero;
    [SerializeField]
    RobotControl RoboCon;
    [SerializeField]
    Text txt;
    Vector3 test = new Vector3(0, -1, 0);
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
        const float l_controlValue_x = 2.5f;
        const float l_controlValue_y = 3.5f;


        Vector3[] Acceleration = new Vector3[2];

        OldAcceleration[HandNum] = NowAcceleration[HandNum];

        if (HandNum == 0)
            NowAcceleration[HandNum] = GetInput.AccelerationGet(Hand.Left) - test;
        if (HandNum == 1)
            NowAcceleration[HandNum] = GetInput.AccelerationGet(Hand.Right) - test;
        txt.text = "" + NowAcceleration[0] + "\n" + NowAcceleration[1] + "\n";

        if (!SamplingFlag[HandNum])
            SamplingFlag[HandNum] = Mathf.Abs(NowAcceleration[HandNum].z) >= l_controlValue_x || Mathf.Abs(NowAcceleration[HandNum].y) >= l_controlValue_y;

        if (SamplingFlag[HandNum])
        {
            AccelerationKeep[HandNum, ListCount[HandNum]] = NowAcceleration[HandNum] ;
            ListCount[HandNum]++;

            if (ListCount[HandNum] >= 10)
            {
                Vector3 Vec3 = Vector3.zero;

                for (int i = 0; i < ListCount[HandNum]; i++)
                    Vec3 += AccelerationKeep[HandNum, i];

                for (int i = 0; i < 10; i++)
                    AccelerationKeep[HandNum, i] = Vector3.zero;

                Acceleration[HandNum] = Vec3;
                ListCount[HandNum] = 0;
                SamplingFlag[HandNum] = false;
            }
        }
        if (Acceleration[HandNum].y < Acceleration[HandNum].z)
        {
            if (Acceleration[HandNum].z >= l_controlValue_x)
            {
                RoboCon.Attack(HandNum, RobotControl.Lane.Right);
            }
            else if (Acceleration[HandNum].z <= -l_controlValue_x)
            {
                RoboCon.Attack(HandNum, RobotControl.Lane.Left);
            }
        }
        else
        {
            if (Mathf.Abs(Acceleration[HandNum].y) >= l_controlValue_y)
            {
                RoboCon.Attack(HandNum, RobotControl.Lane.Center);
            }
        }
    }
}
